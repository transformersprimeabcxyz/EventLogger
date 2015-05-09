/**
/// HashTag.Core Library
/// Copyright © 2010-2014
///
/// This module is Copyright © 2010-2014 Steve Powell
/// All rights reserved.
///
/// This library is free software; you can redistribute it and/or
/// modify it under the terms of the Microsoft Public License (Ms-PL)
/// 
/// This library is distributed in the hope that it will be
/// useful, but WITHOUT ANY WARRANTY; without even the implied
/// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
/// PURPOSE.  See theMicrosoft Public License (Ms-PL) License for more
/// details.
///
/// You should have received a copy of the Microsoft Public License (Ms-PL)
/// License along with this library; if not you may 
/// find it here: http://www.opensource.org/licenses/ms-pl.html
///
/// Steve Powell, hashtagdonet@gmail.com
**/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HashTag
{
    public static partial class Extensions
    {
        /// <summary>
        /// Merge all elements (and their attributes) from <paramref name="sourceDoc"/> to <paramref name="targetDoc"/> 
        /// NOTE: Non-default namespaces are ignored. 
        /// </summary>
        /// <param name="targetDoc"></param>
        /// <param name="sourceDoc"></param>
        public static void Merge(this XmlDocument targetDoc, XmlDocument sourceDoc)
        {
            sourceDoc.RootElement().merge(targetDoc);
        }

        private static void merge(this XmlNode sourceNode, XmlDocument targetDocument)
        {
            if (sourceNode == null) return;

            var targetNode = targetDocument.SelectSingleNode(sourceNode.XPath());
            if (targetNode == null) //source node does not exist in target document
            {
                var targetParent = targetDocument.SelectSingleNode(sourceNode.ParentNode.XPath());
                if (targetParent != null) //TODO this is a work around to ignore assemblyBinding xmlns="urn:..." namespace
                {
                    targetParent.AppendAsChildNode(sourceNode);
                }
            }
            if (sourceNode != null && sourceNode.HasChildNodes == true)
            {
                for (int x = 0; x < sourceNode.ChildNodes.Count; x++)
                {
                    var childNode = sourceNode.ChildNodes[x];
                    if (childNode.NodeType == XmlNodeType.Element)
                    {
                        childNode.merge(targetDocument);
                    }
                }
            }
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="targetParentNode"></param>
        /// <param name="nodeToAppend">NOTE: OwningDocument maybe different than <paramref name="targetParentNode"/>.OwningDocument</param>
        public static void AppendAsChildNode(this XmlNode targetParentNode, XmlNode nodeToAppend)
        {
            var newNode = targetParentNode.OwnerDocument.ImportNode(nodeToAppend, false);
            var xp = newNode.XPath();
            if (string.IsNullOrWhiteSpace(nodeToAppend.InnerText) == false)
            {
                newNode.InnerText = nodeToAppend.InnerText;
            }
            if (newNode.Name == "configSections") // .Net framework requires this section to be first element
            {
                targetParentNode.OwnerDocument.RootElement().AddAsFirstChild(newNode);
            }
            else
            {
                targetParentNode.AppendChild(newNode);
            }
        }

        /// <summary>
        /// Add a child to a node but insert it as the first child in the <paramref name="parentNode"/> children
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="nodeForFirstChild">Node to insert as first child. NOTE: must be in same document context as <paramref name="parentNode"/></param>
        /// <returns></returns>
        public static XmlNode AddAsFirstChild(this XmlNode parentNode, XmlNode nodeForFirstChild)
        {
            if (parentNode.HasChildNodes == true)
            {
                var existingFirstChild = parentNode.FirstChild;
                parentNode.InsertBefore(nodeForFirstChild, existingFirstChild);
            }
            else
            {
                parentNode.AppendChild(nodeForFirstChild);
            }
            return nodeForFirstChild;
        }

        /// <summary>
        /// Returns first &lt;element&gt; of an XML document
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static XmlNode RootElement(this XmlDocument doc)
        {
            for (int x = 0; x < doc.ChildNodes.Count; x++)
            {
                var node = doc.ChildNodes[x];
                if (node.NodeType == XmlNodeType.Element)
                {
                    string s = node.Name;
                    return node;
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieve the first element within same generation of <paramref name="node"/>
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Reference to <paramref name="node"/>'s first sibling</returns>
        public static XmlNode FirstSiblingElement(this XmlNode node)
        {
            if (node == null) return null;
            if (node.ParentNode == null) return node;

            for (int x = 0; x < node.ParentNode.ChildNodes.Count; x++)
            {
                var firstNode = node.ParentNode.ChildNodes[x];
                if (firstNode.NodeType == XmlNodeType.Element)
                {
                    return firstNode;
                }
            }
            return node;
        }

        /// <summary>
        /// Return correctly formatted XPath query that identifies this node to <paramref name="node"/>.OwnerDocument.SelectSingleNode(xpath)
        /// </summary>
        /// <param name="node"></param>
        /// <returns>XPath string from root of <paramref name="node"/>'s owning document</returns>
        public static string XPath(this XmlNode node)
        {
            string basePath = "";
            if (node.ParentNode != null && node.NodeType != XmlNodeType.XmlDeclaration && node.NodeType != XmlNodeType.Document)
            {
                if (node.ParentNode.Name != "#document")
                {
                    basePath += node.ParentNode.XPath();
                }
            }

            string attrPath = "";
            if (node.Attributes != null && node.Attributes.Count > 0)
            {

                for (int x = 0; x < node.Attributes.Count; x++)
                {
                    var attr = node.Attributes[x];
                    if (attrPath.Length > 0)
                    {
                        attrPath += " and ";
                    }
                    attrPath += string.Format("@{0}='{1}'", attr.Name, attr.Value);
                }
            }
            string path = node.Name;
            if (attrPath.Length > 0)
            {
                path = string.Format("{0}[{1}]", node.Name, attrPath);
            }

            return string.Format("{0}/{1}", basePath, path);
        }
    }
}