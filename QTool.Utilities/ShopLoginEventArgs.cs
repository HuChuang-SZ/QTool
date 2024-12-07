using System;

namespace QTool
{
    public class ShopLoginEventArgs : EventArgs
    {
        public ShopLoginEventArgs(int shopId)
        {
            ShopId = shopId;
            Time = DateTime.Now;
            Logined = false;
        }

        public int ShopId { get; }

        public DateTime Time { get; }

        public bool Logined { get; set; }
    }
}
