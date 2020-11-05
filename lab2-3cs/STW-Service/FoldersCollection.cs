using System.Collections.Generic;
using System.Configuration;

namespace STW_Service
{
    [ConfigurationCollection(typeof(FolderElement), AddItemName = "Folder")]
    class FoldersCollection : ConfigurationElementCollection, IEnumerable<FolderElement>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FolderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FolderElement)(element)).Name;
        }

        IEnumerator<FolderElement> IEnumerable<FolderElement>.GetEnumerator()
        {
            foreach (FolderElement folder in this)
            {
                yield return folder;
            }
        }

        public FolderElement this[int index]
        {
            get
            {
                return (FolderElement)BaseGet(index);
            }
        }
    }
}
