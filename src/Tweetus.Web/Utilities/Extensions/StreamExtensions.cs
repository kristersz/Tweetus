using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Tweetus.Web.Utilities.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ToByteArray(this Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                byte[] buffer = new byte[16 * 1024];

                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }
    }
}
