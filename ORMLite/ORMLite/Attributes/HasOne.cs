using System;
namespace ORMLite {
	[AttributeUsage(AttributeTargets.Property)]
	public class HasOne : Attribute {

		public readonly String mappedBy;
		public bool lazy;

		public HasOne(String mappedBy, bool lazy = Configuration.LAZY_FETCH_TYPE) {
			this.mappedBy = mappedBy;
			this.lazy = lazy;
		}
	}
}
