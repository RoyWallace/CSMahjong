using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Sprite
{
    [Serializable]
    class BaseEntry<T>
    {
        public string msg;
        public int code;
        public T data;
    }
}
