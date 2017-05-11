using System;
namespace ORMLite {
	public class CriteriaBuilder {


		private String whereQuery = "";
		private String groupBy = null;
		private String having = null;
		private String orederBy = null;
		private String limit = null;
		private bool distinct = false;

		public String Query() {
			return whereQuery;
		}
		public String GroupBy() {
			return groupBy;
		}
		public String Having() {
			return having;
		}
		public String OrederBy() {
			return orederBy;
		}
		public String Limit() {
			return limit;
		}
		public bool Distinct() {
			return distinct;
		}

		public CriteriaBuilder GroupBy(String groupBy) {
			this.groupBy = groupBy;
			return this;
		}
		public CriteriaBuilder Having(String having) {
			this.having = having;
			return this;
		}
		public CriteriaBuilder OrederBy(String orederBy) {
			this.orederBy = orederBy;
			return this;
		}
		public CriteriaBuilder Limit(String limit) {
			this.limit = limit;
			return this;
		}
		public CriteriaBuilder Distinct(bool distinct) {
			this.distinct = distinct;
			return this;
		}

		public CriteriaBuilder IsNull(String key) {
			whereQuery += " " + key + " IS NULL";
			return this;
		}

		public CriteriaBuilder IsNotNull(String key) {
			whereQuery += " " + key + " IS NOT NULL";
			return this;
		}

		public CriteriaBuilder Equals(String key, Object value) {
			whereQuery += " " + key + " = '" + value.ToString() + "'";
			return this;
		}

		public CriteriaBuilder NotEquals(String key, Object value) {
			whereQuery += " " + key + " != '" + value.ToString() + "'";
			return this;
		}

		public CriteriaBuilder GreaterThan(String key, Object value) {
			whereQuery += " " + key + " > '" + value.ToString() + "'";
			return this;
		}

		public CriteriaBuilder GreaterOrEquals(String key, Object value) {
			whereQuery += " " + key + " >= '" + value.ToString() + "'";
			return this;
		}

		public CriteriaBuilder LowerThan(String key, Object value) {
			whereQuery += " " + key + " < '" + value.ToString() + "'";
			return this;
		}

		public CriteriaBuilder LowerOrEquals(String key, Object value) {
			whereQuery += " " + key + " <= '" + value.ToString() + "'";
			return this;
		}

		public CriteriaBuilder Between(String key, Object smallValue, Object largeValue) {
			whereQuery += " " + key + " BETWEEN '" + smallValue.ToString() + "' AND '" + largeValue.ToString() + "'";
			return this;
		}

		public CriteriaBuilder Like(String key, Object value) {
			whereQuery += " " + key + " LIKE '" + value.ToString() + "'";
			return this;
		}

		public CriteriaBuilder InList(String key, Object[] values) {
			String inClause = GenerateInClause(values);
			whereQuery += " " + key + " IN " + inClause;
			return this;
		}

		public CriteriaBuilder NotInList(String key, Object[] values) {
			String inClause = GenerateInClause(values);
			whereQuery += " " + key + " NOT IN " + inClause;
			return this;
		}

		private String GenerateInClause(Object[] values) {
			String inClause = "(";
			for (int i = 0; i < values.Length; i++) {
				inClause += "'" + values[i].ToString() + "'";
				if (i != (values.Length - 1)) {
					inClause += ",";
				}
			}
			inClause += ")";
			return inClause;
		}

		public CriteriaBuilder And() {
			whereQuery += " AND";
			return this;
		}

		public CriteriaBuilder And(CriteriaBuilder expression) {
			whereQuery = "(" + whereQuery + ") AND (" + expression.Query() + ")";
			return this;
		}

		public CriteriaBuilder Or() {
			whereQuery += " OR";
			return this;
		}

		public CriteriaBuilder Or(CriteriaBuilder expression) {
			whereQuery = "(" + whereQuery + ") OR (" + expression.Query() + ")";
			return this;
		}
	}
}
