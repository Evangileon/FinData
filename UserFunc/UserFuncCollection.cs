using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Configuration;
using UserFunc;
using LuaInterface;

namespace UserFunc
{
    public class UserFuncCollection
    {
        //UserFunc.lua_StatePtr instance = 
        private XmlDocument xmldoc;
        private Lua L;
        

        public UserFuncCollection()
        {
            this.checkXML();
            this.L = new Lua();

        }

        private void checkXML()
        {
            this.xmldoc = new XmlDocument();
            if (!File.Exists(".\\userfunc.xml"))
            {
                File.Create(".\\userfunc.xml");
                xmldoc.Load(".\\userfunc.xml");
                //加入XML的声明段落,<?xml version="1.0" encoding="utf-8"?>
                XmlDeclaration xmldecl;
                xmldecl = xmldoc.CreateXmlDeclaration("1.0", "utf-8", null);
                xmldoc.AppendChild(xmldecl);
            }
        }
    }
}
