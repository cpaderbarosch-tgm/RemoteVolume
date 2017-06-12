#pragma once

#include <windows.h>
#include <mmdeviceapi.h>
#include <audiopolicy.h>
#include <endpointvolume.h>

#define SAFE_RELEASE(x) if(x) { x->Release(); x = NULL; } 

class VolumeControlNative {
public:
	static float GetMasterVolume() {
		IAudioEndpointVolume *endpoint = GetMaster();
		if (endpoint == NULL)
			return NULL;

		float level;
		endpoint->GetMasterVolumeLevelScalar(&level);

		return level * 100;
	}

	static float GetApplicationVolume(int pid)
	{
		ISimpleAudioVolume *volume = GetSession(pid);
		if (volume == NULL)
			return NULL;

		float level = NULL;
		volume->GetMasterVolume(&level);

		SAFE_RELEASE(volume);

		return level * 100;
	}

	static bool GetApplicationMute(int pid)
	{
		ISimpleAudioVolume *volume = GetSession(pid);
		if (volume == NULL)
			return NULL;

		BOOL *mute = NULL;
		volume->GetMute(mute);

		SAFE_RELEASE(volume);

		return mute;
	}

	static void SetMasterVolume(float level) {
		IAudioEndpointVolume *endpoint = GetMaster();
		if (endpoint == NULL)
			return;

		endpoint->SetMasterVolumeLevelScalar(level / 100, NULL);

		SAFE_RELEASE(endpoint);
	}

	static void SetApplicationVolume(int pid, float level)
	{
		ISimpleAudioVolume *volume = GetSession(pid);
		if (volume == NULL)
			return;

		volume->SetMasterVolume(level / 100, NULL);

		SAFE_RELEASE(volume);
	}

	static void SetApplicationMute(int pid, bool mute)
	{
		ISimpleAudioVolume *volume = GetSession(pid);
		if (volume == NULL)
			return;

		volume->SetMute(mute, NULL);

		SAFE_RELEASE(volume);
	}

private:
	static IAudioEndpointVolume* GetMaster() {
		HRESULT                 hr;
		IMMDeviceEnumerator     *enumerator = NULL;
		IMMDevice               *device = NULL;
		IAudioEndpointVolume    *endpoint = NULL;

		CoCreateInstance(__uuidof(MMDeviceEnumerator), NULL, CLSCTX_INPROC_SERVER, __uuidof(IMMDeviceEnumerator), (void**)&enumerator);

		enumerator->GetDefaultAudioEndpoint(eRender, eConsole, &device);

		device->Activate(__uuidof(IAudioEndpointVolume), CLSCTX_INPROC_SERVER, NULL, (void**)&endpoint);

		return endpoint;
	}

	static ISimpleAudioVolume* GetSession(int pid) {
		HRESULT                 hr;
		IMMDeviceEnumerator     *enumerator = NULL;
		ISimpleAudioVolume      *volume = NULL;
		IMMDevice               *device = NULL;
		IAudioSessionManager2   *manager = NULL;
		IAudioSessionEnumerator *sessionEnumerator = NULL;
		int                      sessionCount = 0;

		CoCreateInstance(__uuidof(MMDeviceEnumerator), NULL, CLSCTX_ALL,
			__uuidof(IMMDeviceEnumerator), (void**)&enumerator);

		// Get the default device
		enumerator->GetDefaultAudioEndpoint(EDataFlow::eRender,
			ERole::eMultimedia, &device);

		// Get the session 2 manager
		device->Activate(__uuidof(IAudioSessionManager2), CLSCTX_ALL,
			NULL, (void**)&manager);

		// Get the session enumerator
		manager->GetSessionEnumerator(&sessionEnumerator);

		// Get the session count
		sessionEnumerator->GetCount(&sessionCount);

		// Loop through all sessions
		for (int i = 0; i < sessionCount; i++)
		{
			IAudioSessionControl *ctrl = NULL;
			IAudioSessionControl2 *ctrl2 = NULL;
			DWORD processId = 0;

			hr = sessionEnumerator->GetSession(i, &ctrl);

			if (FAILED(hr))
			{
				continue;
			}

			hr = ctrl->QueryInterface(__uuidof(IAudioSessionControl2), (void**)&ctrl2);

			if (FAILED(hr))
			{
				SAFE_RELEASE(ctrl);
				SAFE_RELEASE(ctrl2);
				continue;
			}

			//Identify WMP process
			hr = ctrl2->GetProcessId(&processId);

			if (FAILED(hr))
			{
				SAFE_RELEASE(ctrl);
				SAFE_RELEASE(ctrl2);
				continue;
			}

			if (processId != pid)
			{
				SAFE_RELEASE(ctrl);
				SAFE_RELEASE(ctrl2);
				continue;
			}

			hr = ctrl2->QueryInterface(__uuidof(ISimpleAudioVolume), (void**)&volume);

			if (FAILED(hr))
			{
				SAFE_RELEASE(ctrl);
				SAFE_RELEASE(ctrl2);
				continue;
			}
		}

		return volume;
	}
};