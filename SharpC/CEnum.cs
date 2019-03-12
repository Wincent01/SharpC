using System;

namespace SharpC
{
    public struct CEnum
    {
        public readonly string DefEnum;

        /// <summary>
        /// Generate C enum
        /// TODO: Fix problem with it wanting to also generate a struct.
        /// </summary>
        /// <param name="type">Enum type</param>
        public CEnum(Type type)
        {
            DefEnum = $"enum {type.Name}\n" + "{\n";

            var names = Enum.GetNames(type);
            var values = Enum.GetValues(type);
            var i = 0;
            foreach (int value in values)
            {
                DefEnum += $"\t{names[i]} = {value}{(i == names.Length - 1 ? "" : ",")}\n";
                i++;
            }

            DefEnum += "};\n\n";
        }
    }
}