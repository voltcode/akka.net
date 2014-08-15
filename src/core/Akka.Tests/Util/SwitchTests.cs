﻿using System;
using Akka.TestKit;
using Akka.Util;
using Xunit;

namespace Akka.Tests.Util
{
    
    public class SwitchTests : AkkaSpec
    {
        [Fact]
        public void OnAndOff()
        {
            var s = new Switch(false);
            Assert.True(s.IsOff, "Initially should be off");
            Assert.False(s.IsOn, "Initially should not be on");

            Assert.True(s.SwitchOn(), "Switch on from off should succeed");
            Assert.True(s.IsOn, "Switched on should be on");
            Assert.False(s.IsOff, "Switched on should not be off");

            Assert.False(s.SwitchOn(), "Switch on when already on should not succeed");
            Assert.True(s.IsOn, "Already switched on should be on");
            Assert.False(s.IsOff, "Already switched on should not be off");

            Assert.True(s.SwitchOff(), "Switch off from on should succeed");
            Assert.True(s.IsOff, "Switched off should be off");
            Assert.False(s.IsOn, "Switched off should not be on");

            Assert.False(s.SwitchOff(), "Switch off when already off should not succeed");
            Assert.True(s.IsOff, "Already switched off should be off");
            Assert.False(s.IsOn, "Already switched off should not be on");
        }

        [Fact]
        public void InitiallyOnShouldBeOn()
        {
            var s = new Switch(true);
            Assert.True(s.IsOn, "Switched on should be on");
            Assert.False(s.IsOff, "Switched on should not be off");
        }

        [Fact]
        public void Given_OffSwitch_When_SwitchOn_throws_exception_Then_Should_revert()
        {
            var s = new Switch(false);
            intercept<InvalidOperationException>(() => s.SwitchOn(() => { throw new InvalidOperationException(); }));
            Assert.True(s.IsOff);
            Assert.False(s.IsOn);
        }


        [Fact]
        public void Given_OnSwitch_When_SwitchOff_throws_exception_Then_Should_revert()
        {
            var s = new Switch(true);
            intercept<InvalidOperationException>(() => s.SwitchOff(() => { throw new InvalidOperationException(); }));
            Assert.True(s.IsOn);
            Assert.False(s.IsOff);
        }

        [Fact]
        public void RunActionWithoutLocking()
        {
            var s = new Switch(false);
            var actionRun = false;
            Assert.True(s.IfOff(() => { actionRun = true; }));
            Assert.True(actionRun);
            actionRun = false;
            Assert.False(s.IfOn(() => { actionRun = true; }));
            Assert.False(actionRun);

            s.SwitchOn();
            actionRun = false;
            Assert.True(s.IfOn(() => { actionRun = true; }));
            Assert.True(actionRun);

            actionRun = false;
            Assert.False(s.IfOff(() => { actionRun = true; }));
            Assert.False(actionRun);
        }


        [Fact]
        public void RunActionWithLocking()
        {
            var s = new Switch(false);
            var actionRun = false;
            Assert.True(s.WhileOff(() => { actionRun = true; }));
            Assert.True(actionRun);
            actionRun = false;
            Assert.False(s.WhileOn(() => { actionRun = true; }));
            Assert.False(actionRun);

            s.SwitchOn();
            actionRun = false;
            Assert.True(s.WhileOn(() => { actionRun = true; }));
            Assert.True(actionRun);

            actionRun = false;
            Assert.False(s.WhileOff(() => { actionRun = true; }));
            Assert.False(actionRun);
        }

    }
}