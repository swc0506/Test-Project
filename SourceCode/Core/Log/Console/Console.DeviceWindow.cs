using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace Core.Log
{
    internal partial class ConsoleController
    {
        private class DeviceWindow : ItemInfoWindow
        {
            private List<string> units;

            protected override void OnStart()
            {
                units = new List<string>() {"B", "KB", "MB", "GB", "TB"};
                infos.Add(new ConsoleItemInfo("Profiler Information", ProfilerInformation, true, 3));
                infos.Add(new ConsoleItemInfo("Application Information", ApplicationInformation));
                infos.Add(new ConsoleItemInfo("Path Information", PathInformation));
                infos.Add(new ConsoleItemInfo("System Information", SystemInformation));
                infos.Add(new ConsoleItemInfo("Screen Information", ScreenInformation));
                infos.Add(new ConsoleItemInfo("Graphics Information", GraphicsInformation));
                infos.Add(new ConsoleItemInfo("Input Information", InputInformation, false, 0.1f));
            }

            private void ProfilerInformation(Dictionary<string, object> map)
            {
                map.Add("Mono Used Size", GetByteSizeText(Profiler.GetMonoUsedSizeLong()));
                map.Add("Mono Heap Size", GetByteSizeText(Profiler.GetMonoHeapSizeLong()));
                map.Add("Used Heap Size", GetByteSizeText(Profiler.usedHeapSizeLong));
                map.Add("Temp Allocator Size", GetByteSizeText(Profiler.GetTempAllocatorSize()));
                map.Add("Total Allocated Memory", GetByteSizeText(Profiler.GetTotalAllocatedMemoryLong()));
                map.Add("Total Reserved Memory", GetByteSizeText(Profiler.GetTotalReservedMemoryLong()));
                map.Add("Total Unused Reserved Memory", GetByteSizeText(Profiler.GetTotalUnusedReservedMemoryLong()));
                map.Add("Allocated Memory For Graphics Driver",
                    GetByteSizeText(Profiler.GetAllocatedMemoryForGraphicsDriver()));
                map.Add("Supported", Profiler.supported);
                map.Add("Enabled", Profiler.enabled);
                map.Add("Max Used Memory", GetByteSizeText(Profiler.maxUsedMemory));
                map.Add("Log File", Profiler.logFile);
                map.Add("Area Count", Profiler.areaCount);
            }

            private void ApplicationInformation(Dictionary<string, object> map)
            {
                map.Add("Platform", Application.platform);
                map.Add("System Language", Application.systemLanguage);
                map.Add("Application Version", Application.version);
                map.Add("Product Name", Application.productName);
                map.Add("Company Name", Application.companyName);
                map.Add("Identifier", Application.identifier);
                map.Add("Install Name", Application.installerName);
                map.Add("Internet Reachability", Application.internetReachability);
                map.Add("Target Frame Rate", Application.targetFrameRate);
                map.Add("Splash Screen IsFinished", SplashScreen.isFinished);
                map.Add("Run In Background", Application.runInBackground);
                map.Add("Background Loading Priority", Application.backgroundLoadingPriority);
                map.Add("Is Mobile Platform", Application.isMobilePlatform);
                map.Add("Is Console Platform", Application.isConsolePlatform);
                map.Add("Is Focused", Application.isFocused);
                map.Add("Is Playing", Application.isPlaying);
                map.Add("Is Editor", Application.isEditor);
                map.Add("Unity Version", Application.unityVersion);
                map.Add("Install Mode", Application.installMode);
                map.Add("Sandbox Type", Application.sandboxType);
                map.Add("Is Batch Mode", Application.isBatchMode);
                map.Add("Build Guid", Application.buildGUID);
                map.Add("Cloud Project Id", Application.cloudProjectId);
            }
            
            private void PathInformation(Dictionary<string, object> map)
            {
                map.Add("Data Path", Application.dataPath);
                map.Add("Persistent Data Path", Application.persistentDataPath);
                map.Add("Streaming Assets Path", Application.streamingAssetsPath);
                map.Add("Temporary Cache Path", Application.temporaryCachePath);
                map.Add("Console Log Path", Application.consoleLogPath);
            }

            private void SystemInformation(Dictionary<string, object> map)
            {
                map.Add("Operating System", SystemInfo.operatingSystem);
                map.Add("Operating System Family", SystemInfo.operatingSystemFamily);
                // map.Add("Device Unique ID", SystemInfo.deviceUniqueIdentifier);
                map.Add("Device Name", SystemInfo.deviceName);
                map.Add("Device Type", SystemInfo.deviceType);
                map.Add("Device Model", SystemInfo.deviceModel);
                map.Add("Processor Type", SystemInfo.processorType);
                map.Add("Processor Count", SystemInfo.processorCount);
                map.Add("Processor Frequency", string.Format("{0} MHz", SystemInfo.processorFrequency));
                map.Add("System Memory Size", string.Format("{0} MB", SystemInfo.systemMemorySize));
                map.Add("Battery Status", SystemInfo.batteryStatus);
                map.Add("Battery Level", GetBatteryLevelText(SystemInfo.batteryLevel));
                map.Add("Supports Audio", SystemInfo.supportsAudio);
                map.Add("Supports Location Service", SystemInfo.supportsLocationService);
                map.Add("Supports Accelerometer", SystemInfo.supportsAccelerometer);
                map.Add("Supports Gyroscope", SystemInfo.supportsGyroscope);
                map.Add("Supports Vibration", SystemInfo.supportsVibration);
                map.Add("Genuine", Application.genuine);
                map.Add("Genuine Check Available", Application.genuineCheckAvailable);
            }

            private void ScreenInformation(Dictionary<string, object> map)
            {
                map.Add("Current Resolution", GetResolutionText(Screen.currentResolution));
                map.Add("Screen Width", GetScreenPixelsText(Screen.width));
                map.Add("Screen Height", GetScreenPixelsText(Screen.height));
                map.Add("Screen Orientation", Screen.orientation);
                map.Add("Auto Landscape Left", Screen.autorotateToLandscapeLeft);
                map.Add("Auto Landscape Right", Screen.autorotateToLandscapeRight);
                map.Add("Auto Portrait", Screen.autorotateToPortrait);
                map.Add("Auto Portrait Upside Down", Screen.autorotateToPortraitUpsideDown);
                map.Add("Screen DPI", Screen.dpi.ToString("F2"));
                map.Add("Safe Area", Screen.safeArea);
                map.Add("Sleep Timeout", GetSleepTimeoutText(Screen.sleepTimeout));
                map.Add("Is Full Screen", Screen.fullScreen);
                map.Add("Full Screen Mode", Screen.fullScreenMode);
#if UNITY_2019_2_OR_NEWER
                map.Add("Cutouts", GetCutoutsString(Screen.cutouts));
                map.Add("Brightness", Screen.brightness.ToString("F2"));
#endif
                map.Add("Cursor Visible", Cursor.visible);
                map.Add("Cursor Lock State", Cursor.lockState);
                map.Add("Support Resolutions", GetResolutionsText(Screen.resolutions));
            }

            private void GraphicsInformation(Dictionary<string, object> map)
            {
                map.Add("Device ID", SystemInfo.graphicsDeviceID);
                map.Add("Device Name", SystemInfo.graphicsDeviceName);
                map.Add("Device Vendor ID", SystemInfo.graphicsDeviceVendorID);
                map.Add("Device Vendor", SystemInfo.graphicsDeviceVendor);
                map.Add("Device Type", SystemInfo.graphicsDeviceType);
                map.Add("Device Version", SystemInfo.graphicsDeviceVersion);
                map.Add("Memory Size", string.Format("{0} MB", SystemInfo.graphicsMemorySize));
                map.Add("Multi Threaded", SystemInfo.graphicsMultiThreaded);
                map.Add("Max Texture Size", SystemInfo.maxTextureSize);
                map.Add("Supported Render Target Count", SystemInfo.supportedRenderTargetCount);
                map.Add("Shader Level", GetShaderLevelText(SystemInfo.graphicsShaderLevel));
                map.Add("Global Maximum LOD", Shader.globalMaximumLOD);
                map.Add("Global Render Pipeline", Shader.globalRenderPipeline);
                map.Add("Active Tier", Graphics.activeTier);
                map.Add("Active Color Gamut", Graphics.activeColorGamut);
                map.Add("NPOT Support", SystemInfo.npotSupport);
                map.Add("Copy Texture Support", SystemInfo.copyTextureSupport);
                map.Add("Uses Reversed ZBuffer", SystemInfo.usesReversedZBuffer);
                map.Add("Max Cubemap Size", SystemInfo.maxCubemapSize);
                map.Add("Graphics UV Starts At Top", SystemInfo.graphicsUVStartsAtTop);
                map.Add("Has Hidden Surface Removal On GPU", SystemInfo.hasHiddenSurfaceRemovalOnGPU);
                map.Add("Has Dynamic Uniform Array Indexing In Fragment Shaders",
                    SystemInfo.hasDynamicUniformArrayIndexingInFragmentShaders);
#if UNITY_2019_2_OR_NEWER
                map.Add("Has Mip Max Level", SystemInfo.hasMipMaxLevel);
                map.Add("Preserve Frame Buffer Alpha", Graphics.preserveFramebufferAlpha);
                map.Add("Min Constant Buffer Offset Alignment", SystemInfo.minConstantBufferOffsetAlignment);
#endif
                map.Add("Supports Instancing", SystemInfo.supportsInstancing);
                map.Add("Supports Shadows", SystemInfo.supportsShadows);
                map.Add("Supports Raw Shadow Depth Sampling", SystemInfo.supportsRawShadowDepthSampling);
                map.Add("Supports Compute Shader", SystemInfo.supportsComputeShaders);
                map.Add("Supports Sparse Textures", SystemInfo.supportsSparseTextures);
                map.Add("Supports 3D Textures", SystemInfo.supports3DTextures);
                map.Add("Supports 2D Array Textures", SystemInfo.supports2DArrayTextures);
                map.Add("Supports Motion Vectors", SystemInfo.supportsMotionVectors);
                map.Add("Supports Cubemap Array Textures", SystemInfo.supportsCubemapArrayTextures);
                map.Add("Supports 3D Render Textures", SystemInfo.supports3DRenderTextures);
                map.Add("Supports Texture Wrap Mirror Once", SystemInfo.supportsTextureWrapMirrorOnce);
                map.Add("Supports Async Compute", SystemInfo.supportsAsyncCompute);
                map.Add("Supports Multisampled Textures", SystemInfo.supportsMultisampledTextures);
                map.Add("Supports Async GPU Readback", SystemInfo.supportsAsyncGPUReadback);
                map.Add("Supports 32bits Index Buffer", SystemInfo.supports32bitsIndexBuffer);
                map.Add("Supports Hardware Quad Topology", SystemInfo.supportsHardwareQuadTopology);
                map.Add("Supports Mip Streaming", SystemInfo.supportsMipStreaming);
                map.Add("Supports Multisample Auto Resolve", SystemInfo.supportsMultisampleAutoResolve);
                map.Add("Supports Separated Render Targets Blend", SystemInfo.supportsSeparatedRenderTargetsBlend);
#if UNITY_2019_2_OR_NEWER
                map.Add("Supports Graphics Fence", SystemInfo.supportsGraphicsFence);
                map.Add("Supports Set Constant Buffer", SystemInfo.supportsSetConstantBuffer);
#endif
            }

            private void InputInformation(Dictionary<string, object> map)
            {
                map.Add("Touch Supported", Input.touchSupported);
                map.Add("Touch Pressure Supported", Input.touchPressureSupported);
                map.Add("Stylus Touch Supported", Input.stylusTouchSupported);
                map.Add("Multi Touch Enabled", Input.multiTouchEnabled);
                map.Add("Touch Count", Input.touchCount);
                map.Add("Simulate Mouse With Touches", Input.simulateMouseWithTouches);
                map.Add("Touches", GetTouchesText(Input.touches));
                map.Add("Mouse Position", Input.mousePosition);
                map.Add("Acceleration", Input.acceleration);
                map.Add("Acceleration Event Count", Input.accelerationEventCount);
                map.Add("Acceleration Events", GetAccelerationEventsText(Input.accelerationEvents));
            }

            private string GetByteSizeText(long size)
            {
                int index = 0;
                double value = size;
                while (value >= 1024 && index < units.Count)
                {
                    value /= 1024;
                    index++;
                }

                return string.Format("{0:F2} {1}", value, units[index]);
            }

            private string GetBatteryLevelText(float batteryLevel)
            {
                return batteryLevel < 0f ? "Unavailable" : batteryLevel.ToString("P0");
            }

            private string GetResolutionText(Resolution resolution)
            {
                return string.Format("{0} x {1} @ {2}Hz", resolution.width, resolution.height,
                    resolution.refreshRate);
            }

            private string GetResolutionsText(Resolution[] resolutions)
            {
                int length = resolutions.Length;
                string[] resolutionStrings = new string[length];
                for (int i = 0; i < length; i++)
                {
                    resolutionStrings[i] = GetResolutionText(resolutions[i]);
                }

                return string.Join("; ", resolutionStrings);
            }

            private string GetScreenPixelsText(int pixels)
            {
                float dpi = Screen.dpi == 0 ? 96 : Screen.dpi;
                float inches = pixels / dpi;
                float centimeters = 2.54f * inches;
                return string.Format("{0} px / {1:F2} in / {2:F2} cm", pixels, inches, centimeters);
            }

            private string GetCutoutsString(Rect[] cutouts)
            {
                int length = cutouts.Length;
                string[] cutoutStrings = new string[length];
                for (int i = 0; i < length; i++)
                {
                    cutoutStrings[i] = cutouts[i].ToString();
                }

                return string.Join("; ", cutoutStrings);
            }

            private string GetSleepTimeoutText(int sleepTimeout)
            {
                string des = SleepTimeout.NeverSleep.ToString();
                int index = des.LastIndexOf(".");
                if (index >= 0)
                {
                    des = des.Substring(index + 1);
                }

                return des;
            }

            private string GetShaderLevelText(int shaderLevel)
            {
                return string.Format("Shader Model {0}.{1}", (shaderLevel / 10).ToString(),
                    (shaderLevel % 10).ToString());
            }

            private string GetTouchText(Touch touch)
            {
                return string.Format("position:{0},deltaPos:{1},rawPos:{2},pressure:{3},phase:{4}", touch.position,
                    touch.deltaPosition, touch.rawPosition, touch.pressure, touch.phase);
            }

            private string GetTouchesText(Touch[] touches)
            {
                string[] touchStrings = new string[touches.Length];
                for (int i = 0; i < touches.Length; i++)
                {
                    touchStrings[i] = GetTouchText(touches[i]);
                }

                return string.Join(";", touchStrings);
            }

            private string GetAccelerationEventString(AccelerationEvent accelerationEvent)
            {
                return string.Format("Position:{0},deltaTime:{1}", accelerationEvent.acceleration,
                    accelerationEvent.deltaTime);
            }

            private string GetAccelerationEventsText(AccelerationEvent[] accelerationEvents)
            {
                string[] accelerationEventStrings = new string[accelerationEvents.Length];
                for (int i = 0; i < accelerationEvents.Length; i++)
                {
                    accelerationEventStrings[i] = GetAccelerationEventString(accelerationEvents[i]);
                }

                return string.Join("; ", accelerationEventStrings);
            }
        }
    }
}