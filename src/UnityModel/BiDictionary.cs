using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityModel
{
    class BiDictionary<TFirst, TSecond>
    {
        readonly Dictionary<TFirst, TSecond>  _forward = new Dictionary<TFirst, TSecond>();
        readonly Dictionary<TSecond, TFirst>  _backward = new Dictionary<TSecond, TFirst>();

        public TFirst this[TSecond second]
        {
            get { return _backward[second]; }
            set
            {
                _backward[second] = value;
                _forward[value] = second;
            }
        }

        public TSecond this [TFirst first]
        {
            get
            {
                return _forward[first];
            }
            set
            {
                _forward[first] = value;
                _backward[value] = first;
            }
        }

        public void Clear()
        {
            _forward.Clear();
            _backward.Clear();
        }
    }
}
