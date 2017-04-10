using System;

namespace Infra.Utils {
public static class EnumUtils {

    public static T ParseEnum<T>(string value) {
        return (T)Enum.Parse(typeof(T), value);
    }

    public static bool Contains<T>(string value) {
        return Enum.IsDefined(typeof(T), value);
    }

    public static T[] EnumValues<T>() where T : struct, IComparable, IConvertible, IFormattable {
        return (T[])Enum.GetValues(typeof(T));
    }
}
}
