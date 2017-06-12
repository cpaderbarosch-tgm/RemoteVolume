#pragma once

#include <mmdeviceapi.h>
#include "VolumeControl.cpp"

using namespace System;
using namespace System::IO;
using namespace std;

namespace RemoteVolume
{
	public ref class VolumeControl
	{
	public:
		static float GetMasterVolume() {
			return VolumeControlNative::GetMasterVolume();
		}

		static float GetApplicationVolume(int pid)
		{
			return VolumeControlNative::GetApplicationVolume(pid);
		}

		static bool GetApplicationMute(int pid)
		{
			return VolumeControlNative::GetApplicationMute(pid);
		}

		static void SetMasterVolume(float level) {
			VolumeControlNative::SetMasterVolume(level);
		}

		static void SetApplicationVolume(int pid, float level)
		{
			VolumeControlNative::SetApplicationVolume(pid, level);
		}

		static void SetApplicationMute(int pid, bool mute)
		{
			VolumeControlNative::SetApplicationMute(pid, mute);
		}
	};
}

