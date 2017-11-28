using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Time_and_motion
{
    public class Ball
    {
        public const int VALID_ORDER_DIFF = 1;

        private int orderIndex;

        public Ball (int inOrderIndex)
        {
            orderIndex = inOrderIndex;
        }

        public int GetOrderIndex()
        {
            return orderIndex;
        }
    }
}
