﻿using System;
using System.Drawing;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TagsCloudVisualization.Infrastructure.Algorithm.Curves;

namespace TagsCloudTests
{
    public class SpiralShould
    {
        [Test]
        public void ThrowException_IncorrectDistanceBetweenLoops()
        {
            Action action = () => new Spiral(Point.Empty, 0);
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void ThrowException_IncorrectIncrement()
        {
            Action action = () => new Spiral(Point.Empty, 1, 0);
            action.Should().Throw<ArgumentException>();
        }


        [TestCase(0, 0)]
        [TestCase(-1000, -1000)]
        [TestCase(1000, 1000)]
        public void ReturnCenter_MustMatchEstablished(int expectedCenterX, int expectedCenterY)
        {
            var expectedCenter = new Point(expectedCenterX, expectedCenterY);
            var spiral = new Spiral(expectedCenter);

            spiral.Center.Should().Be(expectedCenter);
        }


        [Test]
        public void ReturnDistanceBetweenLoops_EqualDistanceBetweenLoops()
        {
            var center = new Point(0, 0);
            var spiral = new Spiral(
                center,
                angleIncrement: (float)Math.PI * 2);


            var previsionPoint = spiral.First();
            foreach (var point in spiral.Skip(1).Take(10))
            {
                var distanceBetweenLoops =
                    Math.Sqrt(Math.Pow(previsionPoint.X - point.X, 2)
                              + Math.Pow(previsionPoint.Y - point.Y, 2));
                previsionPoint = point;
                distanceBetweenLoops.Should().BeInRange(1 - 1e-4, 1 + 1e-4);
            }
        }
    }
}