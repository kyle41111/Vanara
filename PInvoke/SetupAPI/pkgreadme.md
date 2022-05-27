﻿![Vanara](https://github.com/dahall/Vanara/raw/master/docs/icons/VanaraHeading.png)
### Vanara.PInvoke.SetupAPI NuGet Package
[![Version](https://img.shields.io/nuget/v/Vanara.PInvoke.SetupAPI?label=NuGet&style=flat-square)](https://github.com/dahall/Vanara/releases)
[![Build status](https://img.shields.io/appveyor/build/dahall/vanara?label=AppVeyor%20build&style=flat-square)](https://ci.appveyor.com/project/dahall/vanara)

PInvoke API (methods, structures and constants) imported from Windows SetupAPI.dll.

### What is Vanara?

[Vanara](https://github.com/dahall/Vanara) is a community project that contains various .NET assemblies which have P/Invoke functions, interfaces, enums and structures from Windows libraries. Each assembly is associated with one or a few tightly related libraries.

### Issues?

First check if it's already fixed by trying the [AppVeyor build](https://ci.appveyor.com/nuget/vanara-prerelease).
If you're still running into problems, file an [issue](https://github.com/dahall/Vanara/issues).

### Included in Vanara.PInvoke.SetupAPI

Functions | Enumerations | Structures
--- | --- | ---
InstallHinfSection<br>SetupAddInstallSectionToDiskSpaceList<br>SetupAddSectionToDiskSpaceList<br>SetupAddToDiskSpaceList<br>SetupAddToSourceList<br>SetupAdjustDiskSpaceList<br>SetupBackupError<br>SetupCancelTemporarySourceList<br>SetupCloseFileQueue<br>SetupCloseInfFile<br>SetupCloseLog<br>SetupCommitFileQueue<br>SetupConfigureWmiFromInfSection<br>SetupCopyError<br>SetupCopyOEMInf<br>SetupCreateDiskSpaceList<br>SetupDecompressOrCopyFile<br>SetupDefaultQueueCallback<br>SetupDeleteError<br>SetupDestroyDiskSpaceList<br>SetupDiAskForOEMDisk<br>SetupDiBuildClassInfoList<br>SetupDiBuildClassInfoListEx<br>SetupDiBuildDriverInfoList<br>SetupDiCallClassInstaller<br>SetupDiCancelDriverInfoSearch<br>SetupDiChangeState<br>SetupDiClassGuidsFromName<br>SetupDiClassGuidsFromNameEx<br>SetupDiClassNameFromGuid<br>SetupDiClassNameFromGuidEx<br>SetupDiCreateDeviceInfo<br>SetupDiCreateDeviceInfoList<br>SetupDiCreateDeviceInfoListEx<br>SetupDiCreateDeviceInterface<br>SetupDiCreateDeviceInterfaceRegKey<br>SetupDiCreateDevRegKey<br>SetupDiDeleteDeviceInfo<br>SetupDiDeleteDeviceInterfaceData<br>SetupDiDeleteDeviceInterfaceRegKey<br>SetupDiDeleteDevRegKey<br>SetupDiDestroyClassImageList<br>SetupDiDestroyDeviceInfoList<br>SetupDiDestroyDriverInfoList<br>SetupDiDrawMiniIcon<br>SetupDiEnumDeviceInfo<br>SetupDiEnumDeviceInterfaces<br>SetupDiEnumDriverInfo<br>SetupDiGetActualModelsSection<br>SetupDiGetActualSectionToInstall<br>SetupDiGetActualSectionToInstallEx<br>SetupDiGetClassBitmapIndex<br>SetupDiGetClassDescription<br>SetupDiGetClassDescriptionEx<br>SetupDiGetClassDevPropertySheets<br>SetupDiGetClassDevs<br>SetupDiGetClassDevsEx<br>SetupDiGetClassImageIndex<br>SetupDiGetClassImageList<br>SetupDiGetClassImageListEx<br>SetupDiGetClassInstallParams<br>SetupDiGetClassPropertyExW<br>SetupDiGetClassPropertyKeys<br>SetupDiGetClassPropertyKeysExW<br>SetupDiGetClassPropertyW<br>SetupDiGetClassRegistryProperty<br>SetupDiGetCustomDeviceProperty<br>SetupDiGetDeviceInfoListClass<br>SetupDiGetDeviceInfoListDetail<br>SetupDiGetDeviceInstallParams<br>SetupDiGetDeviceInstanceId<br>SetupDiGetDeviceInterfaceAlias<br>SetupDiGetDeviceInterfaceDetail<br>SetupDiGetDeviceInterfacePropertyKeys<br>SetupDiGetDeviceInterfacePropertyW<br>SetupDiGetDevicePropertyKeys<br>SetupDiGetDevicePropertyW<br>SetupDiGetDeviceRegistryProperty<br>SetupDiGetDriverInfoDetail<br>SetupDiGetDriverInstallParams<br>SetupDiGetHwProfileFriendlyName<br>SetupDiGetHwProfileFriendlyNameEx<br>SetupDiGetHwProfileList<br>SetupDiGetHwProfileListEx<br>SetupDiGetINFClass<br>SetupDiGetSelectedDevice<br>SetupDiGetSelectedDriver<br>SetupDiInstallClass<br>SetupDiInstallClassEx<br>SetupDiInstallDevice<br>SetupDiInstallDeviceInterfaces<br>SetupDiInstallDriverFiles<br>SetupDiLoadClassIcon<br>SetupDiLoadDeviceIcon<br>SetupDiOpenClassRegKey<br>SetupDiOpenClassRegKeyEx<br>SetupDiOpenDeviceInfo<br>SetupDiOpenDeviceInterface<br>SetupDiOpenDeviceInterfaceRegKey<br>SetupDiOpenDevRegKey<br>SetupDiRegisterCoDeviceInstallers<br>SetupDiRegisterDeviceInfo<br>SetupDiRemoveDevice<br>SetupDiRemoveDeviceInterface<br>SetupDiRestartDevices<br>SetupDiSelectBestCompatDrv<br>SetupDiSelectDevice<br>SetupDiSelectOEMDrv<br>SetupDiSetClassInstallParams<br>SetupDiSetClassPropertyExW<br>SetupDiSetClassPropertyW<br>SetupDiSetClassRegistryProperty<br>SetupDiSetDeviceInstallParams<br>SetupDiSetDeviceInterfaceDefault<br>SetupDiSetDeviceInterfacePropertyW<br>SetupDiSetDevicePropertyW<br>SetupDiSetDeviceRegistryProperty<br>SetupDiSetDriverInstallParams<br>SetupDiSetSelectedDevice<br>SetupDiSetSelectedDriver<br>SetupDiUnremoveDevice<br>SetupDuplicateDiskSpaceList<br>SetupEnumInfSections<br>SetupFindFirstLine<br>SetupFindNextLine<br>SetupFindNextMatchLine<br>SetupFreeSourceList<br>SetupGetBinaryField<br>SetupGetFieldCount<br>SetupGetFileCompressionInfo<br>SetupGetFileCompressionInfoEx<br>SetupGetFileQueueCount<br>SetupGetFileQueueFlags<br>SetupGetInfDriverStoreLocation<br>SetupGetInfFileList<br>SetupGetInfInformation<br>SetupGetInfPublishedName<br>SetupGetIntField<br>SetupGetLineByIndex<br>SetupGetLineCount<br>SetupGetLineText<br>SetupGetMultiSzField<br>SetupGetNonInteractiveMode<br>SetupGetSourceFileLocation<br>SetupGetSourceFileSize<br>SetupGetSourceInfo<br>SetupGetStringField<br>SetupGetTargetPath<br>SetupGetThreadLogToken<br>SetupInitDefaultQueueCallback<br>SetupInitDefaultQueueCallbackEx<br>SetupInitializeFileLog<br>SetupInstallFile<br>SetupInstallFileEx<br>SetupInstallFilesFromInfSection<br>SetupInstallFromInfSection<br>SetupInstallServicesFromInfSection<br>SetupInstallServicesFromInfSectionEx<br>SetupIterateCabinet<br>SetupLogError<br>SetupLogFile<br>SetupOpenAppendInfFile<br>SetupOpenFileQueue<br>SetupOpenInfFile<br>SetupOpenLog<br>SetupOpenMasterInf<br>SetupPromptForDisk<br>SetupPromptReboot<br>SetupQueryDrivesInDiskSpaceList<br>SetupQueryFileLog<br>SetupQueryInfFileInformation<br>SetupQueryInfOriginalFileInformation<br>SetupQueryInfVersionInformation<br>SetupQuerySourceList<br>SetupQuerySpaceRequiredOnDrive<br>SetupQueueCopy<br>SetupQueueCopyIndirect<br>SetupQueueCopySection<br>SetupQueueDefaultCopy<br>SetupQueueDelete<br>SetupQueueDeleteSection<br>SetupQueueRename<br>SetupQueueRenameSection<br>SetupRemoveFileLogEntry<br>SetupRemoveFromDiskSpaceList<br>SetupRemoveFromSourceList<br>SetupRemoveInstallSectionFromDiskSpaceList<br>SetupRemoveSectionFromDiskSpaceList<br>SetupRenameError<br>SetupScanFileQueue<br>SetupSetDirectoryId<br>SetupSetDirectoryIdEx<br>SetupSetFileQueueAlternatePlatform<br>SetupSetFileQueueFlags<br>SetupSetNonInteractiveMode<br>SetupSetPlatformPathOverride<br>SetupSetSourceList<br>SetupSetThreadLogToken<br>SetupTermDefaultQueueCallback<br>SetupTerminateFileLog<br>SetupUninstallNewlyCopiedInfs<br>SetupUninstallOEMInf<br>SetupVerifyInfFile<br>SetupWriteTextLog<br>SetupWriteTextLogError<br>SetupWriteTextLogInfLine<br> | DEVPROPSTORE<br>DEVPROPTYPE<br>CopyStyle<br>DI_FLAGS<br>DI_FLAGSEX<br>DI_FUNCTION<br>DI_REMOVEDEVICE<br>DI_UNREMOVEDEVICE<br>DIBCI<br>DICD<br>DICLASSPROP<br>DICS<br>DICS_FLAG<br>DICUSTOMDEVPROP<br>DIGCDP_FLAG<br>DIGCF<br>DIOCR<br>DIOD<br>DIODI<br>DIREG<br>DMI<br>DNF<br>FILE_COMPRESSION<br>FILEOP<br>FILEOP_RESULT<br>IDF<br>INF_STYLE<br>INFINFO<br>SCWMI<br>SP_COPY<br>SPCRP<br>SPDIT<br>SPDRP<br>SPDSL<br>SPFILELOG<br>SPFILENOTIFY<br>SPINST<br>SPINT<br>SPPSR<br>SPQ_FLAG<br>SPRDI<br>SPREG<br>SRCINFO<br>SRCLIST<br>LogSeverity<br>SETDIRID<br>SetupFileLogInfo<br>SPLOGFILE<br>SPQ_SCAN<br>SPSVCINST<br>SUOI<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br> | DEVPROPCOMPKEY<br>DEVPROPERTY<br>DEVPROPKEY<br>CABINET_INFO<br>FILE_IN_CABINET_INFO<br>FILEPATHS<br>FILEPATHS_SIGNERINFO<br>HDEVINFO<br>HDSKSPC<br>HINF<br>HSPFILEQ<br>INFCONTEXT<br>SOURCE_MEDIA<br>SP_ALTPLATFORM_INFO_V1<br>SP_ALTPLATFORM_INFO_V2<br>SP_ALTPLATFORM_INFO_V3<br>SP_CLASSIMAGELIST_DATA<br>SP_CLASSINSTALL_HEADER<br>SP_DETECTDEVICE_PARAMS<br>SP_DEVICE_INTERFACE_DATA<br>SP_DEVICE_INTERFACE_DETAIL_DATA<br>SP_DEVINFO_DATA<br>SP_DEVINFO_LIST_DETAIL_DATA<br>SP_DEVINSTALL_PARAMS<br>SP_DRVINFO_DATA_V2<br>SP_DRVINFO_DETAIL_DATA<br>SP_DRVINSTALL_PARAMS<br>SP_FILE_COPY_PARAMS<br>SP_INF_INFORMATION<br>SP_INF_SIGNER_INFO_V1<br>SP_INF_SIGNER_INFO_V2<br>SP_NEWDEVICEWIZARD_DATA<br>SP_ORIGINAL_FILE_INFO<br>SP_POWERMESSAGEWAKE_PARAMS<br>SP_PROPCHANGE_PARAMS<br>SP_PROPSHEETPAGE_REQUEST<br>SP_REGISTER_CONTROL_STATUS<br>SP_REMOVEDEVICE_PARAMS<br>SP_SELECTDEVICE_PARAMS<br>SP_TROUBLESHOOTER_PARAMS<br>SP_UNREMOVEDEVICE_PARAMS<br>HSPFILELOG<br>SP_ALTPLATFORM_INFO<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>