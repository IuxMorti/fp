﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TagsCloudVisualization.Infrastructure;
using TagsCloudVisualization.Infrastructure.Analyzer;
using TagsCloudVisualization.Settings;

namespace TagsCloudTests
{
    public class AnalyzerShould
    {
        private readonly string[] boring = { "а", "перед", "что", "и", "я", "он", "ты", "они" };
        private readonly string[] nouns = { "СОЛОМА, МУДРОСТЬ" };
        private readonly string[] verbs = { "удаляет", "смотрит", "наблюдает" };
        private Analyzer analyzer;
        private AnalyzerSettings settings;


        [SetUp]
        public void SetUp()
        {
            settings = new AnalyzerSettings
            {
                ExcludedParts = new List<PartSpeech>()
            };
            analyzer = new Analyzer(settings);
        }

        [Test]
        public void ChangeCase()
        {
            var result = analyzer.CreateWordFrequenciesSequence(nouns);

            result.GetValueOrThrow().Select(w => w.Word).Should()
                .BeEquivalentTo(nouns.Select(s => s.ToLower()));
        }

        [Test]
        public void CreateFrequencyDictionary()
        {
            var words = new[] { "Привет", "привет", "ПрИвЕт" };

            var result = analyzer.CreateWordFrequenciesSequence(words);

            result.GetValueOrThrow()
                .First()
                .Should().BeEquivalentTo(new WeightedWord { Weight = 3, Word = "привет" });
        }


        [Test]
        public void NotThrowExceptionWhenEmptyLinesExist()
        {
            var words = Enumerable.Repeat("", 100);

            Action action = () => analyzer.CreateWordFrequenciesSequence(words);

            action.Should().NotThrow();
        }


        [Test]
        public void SkipEmpty()
        {
            var words = Enumerable.Repeat("", 100);

            var result = analyzer.CreateWordFrequenciesSequence(words);

            result.GetValueOrThrow().Should().BeEmpty();
        }


        [Test]
        public void ExcludeBoringWords()
        {
            settings.ExcludedParts = new List<PartSpeech>
            {
                PartSpeech.Preposition,
                PartSpeech.Pronoun,
                PartSpeech.Interjection,
                PartSpeech.Particle,
                PartSpeech.Unknown
            };

            var result = analyzer.CreateWordFrequenciesSequence(boring);

            result.GetValueOrThrow().Should().BeEmpty();
        }

        [Test]
        public void ChoosePartsSpeech()
        {
            var words = nouns.Concat(verbs);

            settings.SelectedParts.Add(PartSpeech.Noun);

            var result = analyzer.CreateWordFrequenciesSequence(words);

            result.GetValueOrThrow().Select(w => w.Word)
                .Should()
                .BeEquivalentTo(nouns.Select(s => s.ToLower()));
        }

        [Test]
        public void Lemmatization()
        {
            settings.Lemmatization = true;

            var result = analyzer.CreateWordFrequenciesSequence(verbs);

            result.GetValueOrThrow().Select(w => w.Word)
                .Should().BeEquivalentTo("удалять", "смотреть", "наблюдать");
        }
    }
}