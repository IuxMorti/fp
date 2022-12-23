﻿using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TagsCloudVisualization.Infrastructure.Parsers;
using TagsCloudVisualization.Settings;

namespace TagsCloudTests
{
    public class TxtParserShould
    {
        public const string FileName = "READ_TEST.txt";
        private FileStream fileStream;
        private ParserSettings settings;

        [SetUp]
        public void SetUp()
        {
            settings = new ParserSettings();
            fileStream = File.Create(FileName);
        }

        [TearDown]
        public void TearDown()
        {
            fileStream.Close();
            File.Delete(FileName);
        }


        [TestCase(EncodingEnum.Utf8)]
        [TestCase(EncodingEnum.Utf32)]
        [TestCase(EncodingEnum.Unicode)]
        public void ReadOneWordOneLineText(EncodingEnum encoding)
        {
            var words = string.Join(Environment.NewLine, "привет", "привет");
            var buffer = new TxtParserHelper().Encodings[encoding].GetBytes(words);
            fileStream.Write(buffer, 0, buffer.Length);
            fileStream.Close();
            settings.Encoding = encoding;
            settings.TextType = TextType.OneWordOneLine;
            var parser = new TxtParser(settings);

            var result = parser.WordParse(FileName).GetValueOrThrow().ToArray();

            result.Should().BeEquivalentTo(words.Split(Environment.NewLine));
        }

        [TestCase(EncodingEnum.Utf8)]
        [TestCase(EncodingEnum.Utf32)]
        [TestCase(EncodingEnum.Unicode)]
        public void ReadLiteraryText(EncodingEnum encoding)
        {
            var words = @"- Скажи-ка, дядя, ведь не даром
            Москва
            ";
            var expected = new[] { "Скажи", "ка", "дядя", "ведь", "не", "даром", "Москва" };
            var buffer = new TxtParserHelper().Encodings[encoding].GetBytes(words);
            fileStream.Write(buffer, 0, buffer.Length);
            fileStream.Close();
            settings.Encoding = encoding;
            settings.TextType = TextType.LiteraryText;
            var parser = new TxtParser(settings);

            var result = parser.WordParse(FileName).GetValueOrThrow().ToArray();

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ReturnFail_WhenPathNotExist()
        {
            var path = "1234.txt";
            var parser = new TxtParser(settings);

            var result = parser.WordParse(path);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain(path);
        }

    }
}