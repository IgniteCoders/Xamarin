using System;
namespace ORMLite {
	[AttributeUsage(AttributeTargets.Property)]
	public class Constraints : Attribute {

	    public bool nullable;
	    public bool unique;
	    public bool email;
	    public int maxSize;
	    public int minSize;

		public Constraints() {

		}

		public Constraints(bool nullable = true, bool unique = false, bool email = false, int maxSize = 256, int minSize = 0) {
			this.nullable = nullable;
			this.unique = unique;
			this.email = email;
			this.maxSize = maxSize;
			this.minSize = minSize;
		}
	}
}
