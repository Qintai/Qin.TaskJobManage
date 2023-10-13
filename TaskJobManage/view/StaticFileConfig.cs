namespace Qin.TaskJobManage.view
{
    public class StaticFileConfig
    {
        public StaticFileConfig(ItemGroup myObject)
        {
            StaticFile = myObject;
        }

        public ItemGroup StaticFile { get; set; }
    }

    // 注意: 生成的代码可能至少需要 .NET Framework 4.5 或 .NET Core/Standard 2.0。
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class ItemGroup
    {

        private ItemGroupEmbeddedResource[] embeddedResourceField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("EmbeddedResource")]
        public ItemGroupEmbeddedResource[] EmbeddedResource
        {
            get
            {
                return this.embeddedResourceField;
            }
            set
            {
                this.embeddedResourceField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class ItemGroupEmbeddedResource
    {

        private string includeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Include
        {
            get
            {
                return this.includeField;
            }
            set
            {
                this.includeField = value;
            }
        }
    }

}
