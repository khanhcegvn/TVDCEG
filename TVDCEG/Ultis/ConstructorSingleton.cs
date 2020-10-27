using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVDCEG.Ultis
{
    public abstract class ConstructorSingleton<T> where T:class, new()
    {
        private static T _instance;
        public static T Instance
        {
            get => _instance ?? (_instance = new T());

            set => _instance = value;
        }
    }
}
