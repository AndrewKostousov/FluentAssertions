﻿using System;
using System.Collections.Generic;

namespace FluentAssertions.Assertions
{
    /// <summary>
    /// Provides methods for asserting that two <see cref="DateTime"/> objects differ in certain ways.
    /// </summary>
    public class TimeSpanAssertions
    {
        #region Private Definitions

        private readonly DateTimeAssertions parentAssertions;
        private readonly DateTime? subject;
        private readonly TimeSpan timeSpan;
        private readonly TimeSpanPredicate predicate;

        private readonly Dictionary<TimeSpanCondition, TimeSpanPredicate> predicates = new Dictionary<TimeSpanCondition, TimeSpanPredicate>
        {
            { TimeSpanCondition.MoreThan, new TimeSpanPredicate((ts1, ts2) => ts1 > ts2, "more than") },
            { TimeSpanCondition.AtLeast, new TimeSpanPredicate((ts1, ts2) => ts1 >= ts2, "at least") },
            { TimeSpanCondition.Exactly, new TimeSpanPredicate((ts1, ts2) => ts1 == ts2, "exactly") },
            { TimeSpanCondition.Within, new TimeSpanPredicate((ts1, ts2) => ts1 <= ts2, "within") },  
            { TimeSpanCondition.LessThan, new TimeSpanPredicate((ts1, ts2) => ts1 < ts2, "less than") }    
        };

        #endregion

        protected internal TimeSpanAssertions(DateTimeAssertions parentAssertions, DateTime? subject, TimeSpanCondition condition, TimeSpan timeSpan)
        {
            this.parentAssertions = parentAssertions;
            this.subject = subject;
            this.timeSpan = timeSpan;

            predicate = predicates[condition];
        }

        /// <summary>
        /// Asserts that a <see cref="DateTime"/> occurs a specified amount of time before another <see cref="DateTime"/>.
        /// </summary>
        /// <param name="target">
        /// The <see cref="DateTime"/> to compare the subject with.
        /// </param>
        public AndConstraint<DateTimeAssertions> Before(DateTime target)
        {
            return Before(target, string.Empty);
        }

        /// <summary>
        /// Asserts that a <see cref="DateTime"/> occurs a specified amount of time before another <see cref="DateTime"/>.
        /// </summary>
        /// <param name="target">
        /// The <see cref="DateTime"/> to compare the subject with.
        /// </param>
        /// <param name="reason">
        /// A formatted phrase explaining why the assertion should be satisfied. If the phrase does not 
        /// start with the word <i>because</i>, it is prepended to the message.
        /// </param>
        /// <param name="reasonArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        public AndConstraint<DateTimeAssertions> Before(DateTime target, string reason, params object[] reasonArgs)
        {
            var actual = target.Subtract(subject.Value);

            if (!predicate.IsMatchedBy(actual, timeSpan))
            {
                Execute.Fail("Expected date and/or time {1} to be " + predicate.DisplayText + " {3} before {0}{2}, but it differs {4}.", target, subject,
                    reason, reasonArgs, timeSpan, actual);
            }
            
            return new AndConstraint<DateTimeAssertions>(parentAssertions);
        }

        /// <summary>
        /// Asserts that a <see cref="DateTime"/> occurs a specified amount of time after another <see cref="DateTime"/>.
        /// </summary>
        /// <param name="target">
        /// The <see cref="DateTime"/> to compare the subject with.
        /// </param>
        public AndConstraint<DateTimeAssertions> After(DateTime target)
        {
            return After(target, string.Empty);
        }

        /// <summary>
        /// Asserts that a <see cref="DateTime"/> occurs a specified amount of time after another <see cref="DateTime"/>.
        /// </summary>
        /// <param name="target">
        /// The <see cref="DateTime"/> to compare the subject with.
        /// </param>
        /// <param name="reason">
        /// A formatted phrase explaining why the assertion should be satisfied. If the phrase does not 
        /// start with the word <i>because</i>, it is prepended to the message.
        /// </param>
        /// <param name="reasonArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        public AndConstraint<DateTimeAssertions> After(DateTime target, string reason, params object[] reasonArgs)
        {
            var actual = subject.Value.Subtract(target);

            if (!predicate.IsMatchedBy(actual, timeSpan))
            {
                Execute.Fail("Expected date and/or time {1} to be " + predicate.DisplayText + " {3} after {0}{2}, but it differs {4}.", target, subject,
                    reason, reasonArgs, timeSpan, actual);
            }

            return new AndConstraint<DateTimeAssertions>(parentAssertions);
        }

        /// <summary>
        /// Provides the logic and the display text for a <see cref="TimeSpanCondition"/>.
        /// </summary>
        private class TimeSpanPredicate
        {
            private readonly Func<TimeSpan, TimeSpan, bool> lambda;
            private readonly string displayText;

            public TimeSpanPredicate(Func<TimeSpan, TimeSpan, bool> lambda, string displayText)
            {
                this.lambda = lambda;
                this.displayText = displayText;
            }

            public string DisplayText
            {
                get { return displayText; }
            }

            public bool IsMatchedBy(TimeSpan actual, TimeSpan expected)
            {
                return lambda(actual, expected);
            }
        }
    }
}