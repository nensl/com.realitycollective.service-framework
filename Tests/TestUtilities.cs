﻿// Copyright (c) Reality Collective. All rights reserved.

using NUnit.Framework;
using RealityToolkit.ServiceFramework.Definitions;
using RealityToolkit.ServiceFramework.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.SceneManagement;
using UnityEngine;
using XRTK.Extensions;

namespace RealityToolkit.ServiceFramework.Tests.Utilities
{
    public static class TestUtilities
    {
       public static void InitializeServiceManager(ref ServiceManager serviceManager)
        {
            if (serviceManager == null)
            {
                serviceManager = new ServiceManager();
                serviceManager.Initialize();
            }
            serviceManager.ConfirmInitialized();
        }

        public static void CleanupScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        }

        public static void InitializeServiceManagerScene(ServiceManager serviceManager = null, bool useDefaultProfile = false)
        {
            // Setup
            CleanupScene();
            InitializeServiceManager(ref serviceManager);
            Assert.AreEqual(0, serviceManager.ActiveServices.Count);

            // Tests
            Assert.IsTrue(serviceManager.IsInitialized);
            Assert.IsNotNull(serviceManager);
            Assert.IsFalse(serviceManager.HasActiveProfile);

            ServiceProvidersProfile configuration;

            if (useDefaultProfile)
            {
                configuration = GetDefaultServiceManagerProfile<ServiceProvidersProfile>();
            }
            else
            {
                configuration = ScriptableObject.CreateInstance<ServiceProvidersProfile>();
            }

            Assert.IsTrue(configuration != null, "Failed to find the Service Manager Profile");
            serviceManager.ResetProfile(configuration);
            Assert.IsTrue(serviceManager.ActiveProfile != null);
            Assert.IsTrue(serviceManager.IsInitialized);
        }
        
        public static T RunAsyncMethodSync<T>(Func<Task<T>> asyncFunc) {
            return Task.Run(async () => await asyncFunc()).GetAwaiter().GetResult();
        }
        public static void RunAsyncMethodSync(Func<Task> asyncFunc) {
            Task.Run(async () => await asyncFunc()).GetAwaiter().GetResult();
        }

        private static T GetDefaultServiceManagerProfile<T>() where T : BaseProfile
        {
            return ScriptableObjectExtensions.GetAllInstances<T>().FirstOrDefault(profile => profile.name.Equals(typeof(T).Name));
        }  
    }
}