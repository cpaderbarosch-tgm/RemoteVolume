#pragma once

#include "Windows.h"
#include "Mmdeviceapi.h"
#include "Audiopolicy.h"

#define SAFE_RELEASE(x) if(x) { x->Release(); x = NULL; } 

class VolumeControlNative {
public:
	static float GetApplicationVolume(int pid)
	{
		ISimpleAudioVolume *volume = GetVolumeObject2(pid);
		if (volume == NULL)
			return NULL;

		float level = NULL;
		volume->GetMasterVolume(&level);

		SAFE_RELEASE(volume);

		return level * 100;
	}

	static bool GetApplicationMute(int pid)
	{
		ISimpleAudioVolume *volume = GetVolumeObject2(pid);
		if (volume == NULL)
			return NULL;

		BOOL *mute = NULL;
		volume->GetMute(mute);

		SAFE_RELEASE(volume);

		return mute;
	}

	static void SetApplicationVolume(int pid, float level)
	{
		ISimpleAudioVolume *volume = GetVolumeObject2(pid);
		if (volume == NULL)
			return;

		LPCGUID guid = NULL;
		volume->SetMasterVolume(level / 100, guid);

		SAFE_RELEASE(volume);
	}

	static void SetApplicationMute(int pid, bool mute)
	{
		ISimpleAudioVolume *volume = GetVolumeObject2(pid);
		if (volume == NULL)
			return;

		LPCGUID guid = NULL;
		volume->SetMute(mute, guid);

		SAFE_RELEASE(volume);
	}

private:
	static ISimpleAudioVolume* GetVolumeObject(int pid)
	{
		// get the speakers (1st render + multimedia) device
		IMMDeviceEnumerator *deviceEnumerator = NULL;
		IMMDevice *speakers = NULL;
		IAudioSessionManager2 *mgr = NULL;

		CoCreateInstance(__uuidof(MMDeviceEnumerator), NULL, CLSCTX_ALL, __uuidof(IMMDeviceEnumerator), (void**)&deviceEnumerator);

		deviceEnumerator->GetDefaultAudioEndpoint(EDataFlow::eRender, ERole::eMultimedia, &speakers);

		// activate the session manager. we need the enumerator
		speakers->Activate(__uuidof(IAudioSessionManager2), CLSCTX_ALL, NULL, (void**)&mgr);

		// enumerate sessions for on this device
		IAudioSessionEnumerator *sessionEnumerator = NULL;
		mgr->GetSessionEnumerator(&sessionEnumerator);

		int count = 0;
		sessionEnumerator->GetCount(&count);

		ISimpleAudioVolume *volumeControl = NULL;

		for (int i = 0; i < count; i++)
		{
			IAudioSessionControl *ctrl;
			IAudioSessionControl2 *ctrl2;
			sessionEnumerator->GetSession(i, &ctrl);

			ctrl->QueryInterface(__uuidof(IAudioSessionControl2), (void**)&ctrl2);

			DWORD *cpid = NULL;
			ctrl2->GetProcessId(cpid);

			if (*cpid == pid)
			{
				ctrl2->QueryInterface(__uuidof(ISimpleAudioVolume), (void**)&volumeControl);
				break;
			}
			SAFE_RELEASE(ctrl2);
		}

		SAFE_RELEASE(sessionEnumerator);
		SAFE_RELEASE(mgr);
		SAFE_RELEASE(speakers);
		SAFE_RELEASE(deviceEnumerator);

		return volumeControl;
	}

	static ISimpleAudioVolume* GetVolumeObject2(int pid) {
		HRESULT                 hr;
		IMMDeviceEnumerator     *pEnumerator = NULL;
		ISimpleAudioVolume      *pVolume = NULL;
		IMMDevice               *pDevice = NULL;
		IAudioSessionManager2   *pManager = NULL;
		IAudioSessionEnumerator *pSessionEnumerator = NULL;
		int                      sessionCount = 0;

		hr = CoCreateInstance(__uuidof(MMDeviceEnumerator), NULL, CLSCTX_ALL,
			__uuidof(IMMDeviceEnumerator), (void**)&pEnumerator);

		// Get the default device
		hr = pEnumerator->GetDefaultAudioEndpoint(EDataFlow::eRender,
			ERole::eMultimedia, &pDevice);

		// Get the session 2 manager
		hr = pDevice->Activate(__uuidof(IAudioSessionManager2), CLSCTX_ALL,
			NULL, (void**)&pManager);

		// Get the session enumerator
		hr = pManager->GetSessionEnumerator(&pSessionEnumerator);

		// Get the session count
		hr = pSessionEnumerator->GetCount(&sessionCount);

		// Loop through all sessions
		for (int i = 0; i < sessionCount; i++)
		{
			IAudioSessionControl *ctrl = NULL;
			IAudioSessionControl2 *ctrl2 = NULL;
			DWORD processId = 0;

			hr = pSessionEnumerator->GetSession(i, &ctrl);

			if (FAILED(hr))
			{
				continue;
			}

			hr = ctrl->QueryInterface(__uuidof(IAudioSessionControl2), (void**)&ctrl2);

			if (FAILED(hr))
			{
				SAFE_RELEASE(ctrl);
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

			hr = ctrl2->QueryInterface(__uuidof(ISimpleAudioVolume), (void**)&pVolume);

			if (FAILED(hr))
			{
				SAFE_RELEASE(ctrl);
				SAFE_RELEASE(ctrl2);
				continue;
			}
		}

		return pVolume;
	}
};