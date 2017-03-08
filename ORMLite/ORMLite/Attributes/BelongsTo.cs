using System;
namespace ORMLite {
	[AttributeUsage(AttributeTargets.Property)]
	public class BelongsTo : Attribute {

		public readonly String mappedBy;

		public BelongsTo(String mappedBy) {
			this.mappedBy = mappedBy;
		}
	}
}
