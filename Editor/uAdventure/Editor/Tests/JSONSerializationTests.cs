using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using NUnit.Framework;
using UnityEngine;
using uAdventure.Core;

namespace uAdventure.Test
{
    public class JSONSerializationTests
    {
        private const string FlagId = "FlagId";
        private const string VarId = "VarId";
        private const string GlobalStateId = "GlobalStateId";

        private static Conditions conditions;
        private static FlagCondition flagCondition;
        private static VarCondition varCondition;
        private static GlobalStateCondition globalStateCondition;

        [OneTimeSetUp]
        public void ConditionsSetup()
        {
            flagCondition = new FlagCondition(FlagId, 1);
            varCondition = new VarCondition(VarId, 1, 2);
            globalStateCondition = new GlobalStateCondition(GlobalStateId);
            conditions = new Conditions();
            conditions.Add(flagCondition);
            conditions.Add(varCondition);
            conditions.Add(varCondition);
        }

        [Test]
        public void SerializeSingleFlagConditionTest()
        {
            var serializedFlagCondition = JsonUtility.ToJson(flagCondition);
            Debug.Log(serializedFlagCondition);
            var restoredFlagCondition = JsonUtility.FromJson<FlagCondition>(serializedFlagCondition);

            Assert.NotNull(restoredFlagCondition);
            Assert.AreEqual(flagCondition.GetType(), restoredFlagCondition.GetType());
            Assert.AreEqual(flagCondition.getType(), restoredFlagCondition.getType());
            Assert.AreEqual(flagCondition.getState(), restoredFlagCondition.getState());
        }

        [Test]
        public void SerializeSingleVarConditionTest()
        {
            var serializedVarCondition = JsonUtility.ToJson(varCondition);
            Debug.Log(serializedVarCondition);
            var restoredVarCondition = JsonUtility.FromJson<VarCondition>(serializedVarCondition);

            Assert.NotNull(restoredVarCondition);
            Assert.AreEqual(varCondition.GetType(), restoredVarCondition.GetType());
            Assert.AreEqual(varCondition.getType(), restoredVarCondition.getType());
            Assert.AreEqual(varCondition.getState(), restoredVarCondition.getState());
            Assert.AreEqual(varCondition.getValue(), restoredVarCondition.getValue());
        }

        [Test]
        public void SerializeSingleGlobalStateConditionTest()
        {
            var serializedGlobalStateCondition = JsonUtility.ToJson(globalStateCondition);
            Debug.Log(serializedGlobalStateCondition);
            var restoredGlobalStateCondition = JsonUtility.FromJson<GlobalStateCondition>(serializedGlobalStateCondition);

            Assert.NotNull(restoredGlobalStateCondition);
            Assert.AreEqual(globalStateCondition.GetType(), restoredGlobalStateCondition.GetType());
            Assert.AreEqual(globalStateCondition.getType(), restoredGlobalStateCondition.getType());
            Assert.AreEqual(globalStateCondition.getState(), restoredGlobalStateCondition.getState());
        }

        [Test]
        public void SerializeConditionsTest()
        {
            var serializedConditions = JsonUtility.ToJson(conditions);
            Debug.Log(serializedConditions);
            var restoredConditions = JsonUtility.FromJson<Conditions>(serializedConditions);

            Assert.NotNull(restoredConditions);
            Assert.AreEqual(conditions.GetType(), restoredConditions.GetType());
            Assert.AreEqual(conditions.Size(), restoredConditions.Size());

            for (int i = 0; i < conditions.Size(); i++)
            {
                var currentConditions = conditions.Get(i);
                var otherConditions = restoredConditions.Get(i);

                Assert.AreEqual(currentConditions.Count, otherConditions.Count);

                for (int j = 0; j < currentConditions.Count; j++)
                {
                    Assert.AreEqual(currentConditions[j].getType(), otherConditions[j].getType());
                    Assert.AreEqual(currentConditions[j].getState(), otherConditions[j].getState());
                }
            }
        }
    }
}
