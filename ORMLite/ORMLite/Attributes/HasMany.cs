using System;
namespace ORMLite {
	[AttributeUsage(AttributeTargets.Property)]
	public class HasMany : Attribute {

		public readonly String mappedBy;
		public bool lazy;

		public HasMany(String mappedBy, bool lazy = Configuration.LAZY_FETCH_TYPE) {
			this.mappedBy = mappedBy;
			this.lazy = lazy;
		}
	}
}
