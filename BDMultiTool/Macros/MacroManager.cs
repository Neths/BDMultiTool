using BDMultiTool.Persistence;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using BDMultiTool.Core.PInvoke;

namespace BDMultiTool.Macros
{
    public interface IMacroManager
    {
    }

    public class MacroManager : IMacroManager
    {
        private readonly IOverlay _overlay;
        private readonly IWindowAttacher _windowAttacher;
        private readonly ConcurrentDictionary<string, CycleMacro> _macros;
        private MacroGallery macroGallery;

        public MacroManager(IOverlay overlay, IWindowAttacher windowAttacher)
        {
            _overlay = overlay;
            _windowAttacher = windowAttacher;
            _macros = new ConcurrentDictionary<String, CycleMacro>();
            macroGallery = new MacroGallery();
            macroGallery.initialize();


            PersistenceContainer[] savedMacros = PersistenceUnitThread.persistenceUnit.loadContainersByType(typeof(CycleMacro).Name);
            foreach (PersistenceContainer currentPersistenceContainer in savedMacros)
            {
                CycleMacro newMacro = new CycleMacro();
                newMacro.UpdateCycleMacroByPersistenceContainer(currentPersistenceContainer);
                _macros.GetOrAdd(newMacro.Name, newMacro);
            }
        }

        public void addMacro(CycleMacro macroToAdd)
        {
            macroToAdd.Pause();
            _macros.GetOrAdd(macroToAdd.Name, macroToAdd);
            macroToAdd.Persist();
        }

        public void update()
        {
            int tempCount = 0;
            ObservableCollection<MacroItemModel> macroItemModels = new ObservableCollection<MacroItemModel>();
            foreach (KeyValuePair<String, CycleMacro> currentMacroKeyValuePair in _macros)
            {
                tempCount++;
                if (currentMacroKeyValuePair.Value.IsReady())
                {
                    sendMultipleKeys(currentMacroKeyValuePair.Value.GetKeys());
                    currentMacroKeyValuePair.Value.Reset();
                    currentMacroKeyValuePair.Value.Start();
                    Thread.Sleep(20);
                }
                if (currentMacroKeyValuePair.Value.LifeTimeOver())
                {
                    removeMacro(currentMacroKeyValuePair.Value);
                }
                else
                {
                    MacroItemModel currentMacroItemModel = new MacroItemModel();
                    currentMacroItemModel.MacroName = currentMacroKeyValuePair.Value.Name;
                    currentMacroItemModel.CoolDownTime = currentMacroKeyValuePair.Value.GetRemainingCoolDownFormatted();
                    currentMacroItemModel.CoolDownPercentage = currentMacroKeyValuePair.Value.GetCoolDownPercentage();
                    currentMacroItemModel.LifeTime = currentMacroKeyValuePair.Value.GetRemainingLifeTimeFormatted();
                    currentMacroItemModel.LifeTimePercentage = currentMacroKeyValuePair.Value.GetLifeTimePercentage();
                    currentMacroItemModel.KeyString = currentMacroKeyValuePair.Value.GetKeyString();
                    currentMacroItemModel.AddMode = false;

                    if (currentMacroKeyValuePair.Value.Paused)
                    {
                        currentMacroItemModel.Paused = true;
                        currentMacroItemModel.NotPaused = false;
                    }
                    else
                    {
                        currentMacroItemModel.Paused = false;
                        currentMacroItemModel.NotPaused = true;
                    }

                    macroItemModels.Add(currentMacroItemModel);

                    macroGallery.Dispatcher.Invoke((Action)(() =>
                    {
                        bool macroContained = false;
                        foreach (MacroItemModel currentInnerMacroItemModel in macroGallery.macroItemModels)
                        {
                            if (currentInnerMacroItemModel.MacroName == currentMacroItemModel.MacroName)
                            {
                                currentInnerMacroItemModel.CoolDownPercentage = currentMacroItemModel.CoolDownPercentage;
                                currentInnerMacroItemModel.CoolDownTime = currentMacroItemModel.CoolDownTime;
                                currentInnerMacroItemModel.LifeTime = currentMacroItemModel.LifeTime;
                                currentInnerMacroItemModel.LifeTimePercentage = currentMacroItemModel.LifeTimePercentage;
                                macroContained = true;
                                break;
                            }
                        }
                        if (!macroContained)
                        {
                            macroGallery.addMacro(currentMacroItemModel);
                        }
                    }));
                }
            }


        }

        public void removeMacro(CycleMacro macro)
        {
            CycleMacro deletedMacro;
            while (!_macros.TryRemove(macro.Name, out deletedMacro)) { }

            deletedMacro.DeletePersistence();

            macroGallery.Dispatcher.Invoke((Action)(() =>
            {
                foreach (MacroItemModel currentInnerMacroItemModel in macroGallery.macroItemModels)
                {
                    if (currentInnerMacroItemModel.MacroName == deletedMacro.Name)
                    {
                        macroGallery.macroItemModels.Remove(currentInnerMacroItemModel);
                        break;
                    }
                }
            }));

        }

        public void removeMacroByName(String name)
        {
            CycleMacro deletedMacro;
            while (!_macros.TryRemove(name, out deletedMacro)) { }

            deletedMacro.DeletePersistence();

            macroGallery.Dispatcher.Invoke((Action)(() =>
            {
                foreach (MacroItemModel currentInnerMacroItemModel in macroGallery.macroItemModels)
                {
                    if (currentInnerMacroItemModel.MacroName == deletedMacro.Name)
                    {
                        macroGallery.macroItemModels.Remove(currentInnerMacroItemModel);
                        break;
                    }
                }
            }));
        }

        public CycleMacro getMacroByName(String name)
        {
            if (_macros.ContainsKey(name))
            {
                CycleMacro receivedMacro;

                while (!_macros.TryGetValue(name, out receivedMacro)) { }
                return receivedMacro;
            }
            else
            {
                return null;
            }
        }

        private void sendMultipleKeys(System.Windows.Forms.Keys[] keys)
        {
            foreach (System.Windows.Forms.Keys currentKey in keys)
            {
                _windowAttacher.SendKeypress(currentKey);
                Thread.Sleep(10);
            }
        }

        public KeyValuePair<String, CycleMacro>[] getActiveMacros()
        {
            return _macros.ToArray();
        }
    }


}
