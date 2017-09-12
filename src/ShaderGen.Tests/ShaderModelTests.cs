﻿using Microsoft.CodeAnalysis;
using System.IO;
using TestShaders;
using Xunit;

namespace ShaderGen.Tests
{
    public class ShaderModelTests
    {

        [Fact]
        public void TestVertexShader_ShaderModel()
        {
            string functionName = "TestShaders.TestVertexShader.VS";
            Compilation compilation = TestUtil.GetTestProjectCompilation();
            HlslBackend backend = new HlslBackend(compilation);
            ShaderGenerator sg = new ShaderGenerator(
                compilation,
                functionName,
                null,
                backend);
            ShaderModel shaderModel = sg.GenerateShaders();
            Assert.Equal(2, shaderModel.Structures.Length);
            Assert.Equal(3, shaderModel.Resources.Length);
            ShaderFunction vsEntry = shaderModel.GetFunction(functionName);
            Assert.Equal("VS", vsEntry.Name);
            Assert.Equal(1, vsEntry.Parameters.Length);
            Assert.True(vsEntry.IsEntryPoint);
            Assert.Equal(ShaderFunctionType.VertexEntryPoint, vsEntry.Type);
        }

        [Fact]
        public void TestVertexShader_VertexSemantics()
        {
            string functionName = "TestShaders.TestVertexShader.VS";
            Compilation compilation = TestUtil.GetTestProjectCompilation();
            HlslBackend backend = new HlslBackend(compilation);
            ShaderGenerator sg = new ShaderGenerator(
                compilation,
                functionName,
                null,
                backend);
            ShaderModel shaderModel = sg.GenerateShaders();

            StructureDefinition vsInput = shaderModel.GetStructureDefinition(nameof(TestShaders) + "." + nameof(PositionTexture));
            Assert.Equal(SemanticType.Position, vsInput.Fields[0].SemanticType);
            Assert.Equal(SemanticType.TextureCoordinate, vsInput.Fields[1].SemanticType);

            StructureDefinition fsInput = shaderModel.GetStructureDefinition(
                nameof(TestShaders) + "." + nameof(TestVertexShader) + "." + nameof(TestVertexShader.VertexOutput));
            Assert.Equal(SemanticType.Position, fsInput.Fields[0].SemanticType);
            Assert.Equal(SemanticType.TextureCoordinate, fsInput.Fields[1].SemanticType);
        }

        [Fact]
        public void PartialFiles()
        {
            Compilation compilation = TestUtil.GetTestProjectCompilation();
            HlslBackend backend = new HlslBackend(compilation);
            ShaderGenerator sg = new ShaderGenerator(
                compilation,
                "TestShaders.PartialVertex.VertexShader",
                null,
                backend);

            ShaderModel shaderModel = sg.GenerateShaders();
            ShaderFunction entryFunction = shaderModel.GetFunction("VertexShaderFunc");
            string vsCode = backend.GetCode(entryFunction);
            FxcTool.AssertCompilesCode(vsCode, "vs_5_0", entryFunction.Name);
        }
    }
}