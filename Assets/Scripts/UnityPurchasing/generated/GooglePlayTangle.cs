// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("oUCVNbdJvZH4dVKGY2uKKA3X/OP+yoJ/HAv3+BwWb2VpyeDvaGcgot4y33sbWN/y4KZNSxnxtMKsbu11KNjEJPVMMNcRVgCVHaSiOFw0jABA8nFSQH12eVr2OPaHfXFxcXVwc/Jxf3BA8nF6cvJxcXDO+5uSLGC50doEjWzKFFOzvtBw8n3IYXwG/80/bRYPFz8nuMljYrmF9rksZ/LWXjKoGpPhKzmucuUNawNj1rK1a4FK8JtDdGhouudwiOzDLO8ZKVdDuw7U5oyUHhZKnQJikPSj2Vi0TMT8v/OldDTOFUSXsEERUxX7YSNXux4xPwLYHVbdwJecb73LR8ny4tkr1pclaQJSxTH1G8UMKx3pGSdc7XZTXxKrhkPXP3fzKXJzcXBx");
        private static int[] order = new int[] { 6,3,9,13,6,13,9,8,12,11,13,12,12,13,14 };
        private static int key = 112;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
