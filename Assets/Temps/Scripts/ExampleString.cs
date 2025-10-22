using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Text;

namespace PracticalModules.Temps.Scripts
{
    public class YourClass
    {
        public void Foo()
        {
            Debug.Log("Hello, World!");
        }
    }

    public class ExampleString : MonoBehaviour
    {
        private void TestString()
        {
            string formattedString = ZString.Format("Hello {0} {1}", 1, 2);
            string concatenatedString = ZString.Concat("Hello ", "World", "!");

            using (var sb = ZString.CreateUtf8StringBuilder())
            {
                sb.Append("sdfsdf");
                sb.AppendJoin(",", new List<string> { "1", "2", "3" });
                string result = sb.ToString();
            }
        }

        private void TestLogger()
        {
            
        }
    }
}
