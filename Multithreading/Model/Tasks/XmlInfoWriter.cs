using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using FilesTree.Model.Data;

namespace FilesTree.Model.Tasks
{
    /// <summary>
    ///     Class for writing all collected data to XML file.
    /// </summary>
    internal sealed class XmlInfoWriter : QueueableTask<FileSystemObjectData>, IDisposable
    {
        private readonly List<FileSystemObjectData> _nodesHierarchy = new List<FileSystemObjectData>();
        private readonly XmlWriter _xmlWriter;
        private FileSystemObjectData _lastAddedNode;

        public XmlInfoWriter(string outputFilePath)
        {
            var settings = new XmlWriterSettings {CloseOutput = true, Encoding = new UTF8Encoding(), Indent = true};
            _xmlWriter = XmlWriter.Create(outputFilePath, settings);
            _xmlWriter.WriteStartDocument();
        }

        public void Dispose()
        {
            if (_xmlWriter != null)
                _xmlWriter.Close();
        }

        protected override void ProcessDequeuedData(FileSystemObjectData data)
        {
            CloseParentNodes(data);
            WriteNodeStart(data);
            _lastAddedNode = data;
        }

        protected override void FinishTask()
        {
            // Close all opened elements
            CloseXmlElement(_lastAddedNode);
            for (int i = _nodesHierarchy.Count - 1; i >= 0; i--)
                CloseXmlElement(_nodesHierarchy[i]);

            _xmlWriter.WriteEndDocument();
            _xmlWriter.Flush();
            base.FinishTask();
        }

        private void WriteNodeStart(FileSystemObjectData data)
        {
            string elementName = data is FileData ? "File" : "Directory";
            _xmlWriter.WriteStartElement(elementName);
            _xmlWriter.WriteAttributeString("Name", data.Name);

            // Write file system object properties
            _xmlWriter.WriteStartElement("Properties");
            _xmlWriter.WriteAttributeString("CreationTime", data.CreationTime.ToString(CultureInfo.InvariantCulture));
            _xmlWriter.WriteAttributeString("LastWriteTime", data.LastWriteTime.ToString(CultureInfo.InvariantCulture));
            _xmlWriter.WriteAttributeString("LastAccessTime", data.LastAccessTime.ToString(CultureInfo.InvariantCulture));
            _xmlWriter.WriteAttributeString("Owner", data.Owner);
            _xmlWriter.WriteEndElement();

            WriteFlagElements(data.Attributes, "Attributes");
            WriteFlagElements(data.FileSystemRights, "FileSystemRights");
        }

        /// <summary>
        ///     Close all unclosed elements represented in filed _nodesHierarchy.
        /// </summary>
        /// <param name="data">Current element's data.</param>
        private void CloseParentNodes(FileSystemObjectData data)
        {
            // If current element is the first
            if (_lastAddedNode == null)
                return;

            // If last added node is parent of current, then current node have to be inside last added element. In other case close last element.
            if (_lastAddedNode == data.Parent)
            {
                _nodesHierarchy.Add(_lastAddedNode);
                return;
            }
            CloseXmlElement(_lastAddedNode);

            // Close unclosed elements in _nodesHierarchy until one of _nodesHierarchy's elements is parent of current element.
            for (int i = _nodesHierarchy.Count - 1; i >= 0; i--)
            {
                if (_nodesHierarchy[i] == data.Parent)
                    return;
                CloseXmlElement(_nodesHierarchy[i]);
                _nodesHierarchy.RemoveAt(i);
            }
        }

        private void CloseXmlElement(FileSystemObjectData data)
        {
            _xmlWriter.WriteStartElement("TotalSize");
            _xmlWriter.WriteAttributeString("Size", data.BytesSize.ToString(CultureInfo.InvariantCulture));
            _xmlWriter.WriteEndElement();
            _xmlWriter.WriteEndElement();
        }

        /// <summary>
        ///     Add all flag values as one XML element.
        /// </summary>
        private void WriteFlagElements<T>(T flag, string groupName) where T : struct, IComparable, IConvertible, IFormattable
        {
            if (typeof (T).IsSubclassOf(typeof (Enum)) == false)
                throw new ArgumentException("T is not an enum.");

            List<T> flagEnums = Enum.GetValues(typeof (T)).Cast<T>().Where(@enum => (Convert.ToInt64(@enum) & Convert.ToInt64(flag)) != 0).ToList();
            if (!flagEnums.Any())
                return;

            StringBuilder strBuilder = flagEnums.Aggregate(new StringBuilder(), (builder, @enum) => builder.Append(", " + @enum.ToString(CultureInfo.InvariantCulture)),
                builder => builder.Remove(0, 2));
            _xmlWriter.WriteElementString(groupName, strBuilder.ToString());
        }
    }
}