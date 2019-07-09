using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization;

public static  class YamlUtils
{

    public static string ToYaml(object obj)
    {
        if (obj == null)
            return null;
        var yamlSerializer = new SerializerBuilder().Build();
        var yaml = yamlSerializer.Serialize(obj);
        return yaml;
    }

    public static T FromYaml<T>(string yaml)
    {
        if (string.IsNullOrEmpty(yaml))
            return default(T);

        var deserializer = new DeserializerBuilder().Build();
        var yamlObject = deserializer.Deserialize<T>(yaml);
        return yamlObject;
    }

    public static string Json2Yaml(string json)
    {
        var deserializer = new DeserializerBuilder().Build();
        var jsonObject = deserializer.Deserialize(json);
        var yamlSerializer = new SerializerBuilder().Build();
        var yaml = yamlSerializer.Serialize(jsonObject);

        return yaml;
    }

    public static string Yaml2Json(string yaml)
    {
        var deserializer = new DeserializerBuilder().Build();
        var yamlObject = deserializer.Deserialize(yaml);

        var serializer = new SerializerBuilder()
            .JsonCompatible()
            .Build();

        string json = serializer.Serialize(yamlObject);
        return json;
    }
}

