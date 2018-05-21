﻿using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DatabaseSchemaReader.CodeGen;
using Microsoft.CSharp;

namespace DatabaseSchemaReader.DataSchema
{
    public class EnumeratedDataType: DataType
    {
        public List<string> EnumerationValues { get; set; }

        public EnumeratedDataType(string typeName, string netDataType): base(typeName, netDataType) { }

        public override string WriteCodeFile(CodeWriterSettings codeWriterSettings, ClassBuilder classBuilder)
        {
            classBuilder.AppendLine(@"//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by a Tool.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
//
//    Behavior of class members defined in this file may be changed by overriding in a derived class.
// </auto-generated>
//------------------------------------------------------------------------------");
            classBuilder.AppendLine("");
            classBuilder.BeginNest($"namespace {codeWriterSettings.Namespace}");

            classBuilder.AppendLine("using NpgsqlTypes;");
            classBuilder.AppendLine("");
            classBuilder.BeginNest($"public enum {NetDataType}");

            for (var i = 0; i < EnumerationValues.Count(); i++)
            {
                var enumerationValueToWrite = EnumerationValues[i].Replace(" ", "_");
                if (i < EnumerationValues.Count() - 1)
                {
                    enumerationValueToWrite += ",";
                }
                classBuilder.AppendLine($"[PgName(\"{EnumerationValues[i]}\")]");
                classBuilder.AppendLine(enumerationValueToWrite);
            }
            
            classBuilder.EndNest();

            classBuilder.EndNest();

            return classBuilder.ToString();
        }
    }
}
