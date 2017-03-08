using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
namespace ORMLite {
	public abstract class PersistentEntity {

		public long id = -1;

		public virtual dynamic GetTableData() {
			dynamic tableData;
			try {
				FieldInfo property = GetType().GetTypeInfo().GetDeclaredField(Configuration.ACCESS_PROPERTY);
				tableData = property.GetValue(this);
			} catch (Exception e) {
				SQLConsole.WriteLine(e.StackTrace);
				tableData = null;
			}
			return tableData;
		}

		public PersistentEntity() {

		}


	
		public virtual bool save() {
			if (id != -1 && this.GetTableData().Get(id) != null) {
				return Update();
			} else {
				return Insert();
			}
		}

		public virtual bool BeforeInsert() {
			Type superClass = Reflections.GetBaseType(this.GetType());
			if (superClass != typeof(PersistentEntity)) {
				PersistentEntity superObject = ((PersistentEntity)Reflections.GetSuperInstanceFromInstance(this));
				if (superObject.Insert()) {
					this.SetId(superObject.GetId());
					return true;
				} else {
					return false;
				}
	        }
			return true;
		}

		public virtual bool Insert() {
			if (BeforeInsert() == true) {
				long insertId = GetTableData().Insert((dynamic)this);
				if (insertId > -1) {
					this.id = insertId;
					AfterInsert();
					return true;
				} else {
					return false;
				}
			} else {
				return false;
			}
		}

		public virtual void AfterInsert() {
			PerformActionOnChilds(Action.Insert);
		}

		public virtual bool BeforeUpdate() {
			Type superClass = Reflections.GetBaseType(this.GetType());
			if (superClass != typeof(PersistentEntity)) {
				PersistentEntity superObject = ((PersistentEntity)Reflections.GetSuperInstanceFromInstance(this));
				if (superObject.Update()) {
					this.SetId(superObject.GetId());
					return true;
				} else {
					return false;
				}
			}
			return true;
		}

		public virtual bool Update() {
			if (BeforeUpdate() == true) {
				int updateRows = GetTableData().Update((dynamic)this);

				if (updateRows > 0) {
					AfterUpdate();
					return true;
				} else {
					return false;
				}
			} else {
				return false;
			}
		}

		public virtual void AfterUpdate() {
			//PerformActionOnChilds(Action.Update);
		}

		public virtual bool BeforeDelete() {
			PerformActionOnChilds(Action.Delete);
			return true;
		}

		public virtual bool Delete() {
			if (BeforeDelete() == true) {
				int deletedRows = GetTableData().Delete((dynamic)this);
				if (deletedRows > 0) {
					AfterDelete();
					return true;
				} else {
					return false;
				}
			} else {
				return false;
			}
		}

		public virtual void AfterDelete() {
			Type superClass = Reflections.GetBaseType(this.GetType());
			if (superClass != typeof(PersistentEntity)) {
				PersistentEntity superObject = ((PersistentEntity)Reflections.GetSuperInstanceFromInstance(this));
				superObject.Delete();
			}
		}

		public long GetId() {
			return id;
		}

		public void SetId(long id) {
			this.id = id;
		}

		public virtual long GetServerId() {
			return GetId();
		}

		public virtual void SetServerId(long id) {
			SetId(id);
		}

		public override String ToString() {
			return GetTableData().GetTableMapping().name + ": " + this.id;
		}

		private enum Action { Insert, Update, Delete }
		private void PerformActionOnChilds(Action action) {
			ColumnInfo[] fields = GetTableData().GetTableMapping().columns;
			foreach (ColumnInfo field in fields) {
				IList childs = new List<PersistentEntity>();
				String mappedBy = "";
				if (Reflections.IsAttributePresent(field.property, typeof(HasMany))) {
					HasMany hasMany = (HasMany)Reflections.GetAttribute(field.property, typeof(HasMany));
					mappedBy = hasMany.mappedBy;
					childs = (IList)field.property.GetValue(this);
				} else if (Reflections.IsAttributePresent(field.property, typeof(HasOne))) {
					HasOne hasOne = (HasOne)Reflections.GetAttribute(field.property, typeof(HasOne));
					mappedBy = hasOne.mappedBy;
					childs.Add((PersistentEntity)field.property.GetValue(this));
				}
				if (childs != null) {
					foreach (PersistentEntity child in childs) {
						switch (action) {
							case Action.Insert:
								PropertyInfo childField = Reflections.GetDeclaredFieldRecursively(mappedBy, child.GetType(), typeof(PersistentEntity));
								childField.SetValue(child, this);
								child.Insert();
								break;
							case Action.Update:
								child.Update();
								break;
							case Action.Delete:
								child.Delete();
								break;
						}
					}
				}
			}
		}
	}
}
