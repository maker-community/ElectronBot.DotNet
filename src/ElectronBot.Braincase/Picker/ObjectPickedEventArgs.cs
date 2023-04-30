using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronBot.Braincase.Picker
{
    public class ObjectPickedEventArgs<T> : EventArgs
    {
        public T Result { get; set; }

        public ObjectPickedEventArgs(T result)
        {
            Result = result;
        }
    }
}
