using BDMultiTool.Persistence;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BDMultiTool.Core.PInvoke;

namespace BDMultiTool.Macros
{
    public interface IMacroManager
    {
        void showMacroMenu();
    }

    public class MacroManager : IMacroManager
    {
        private readonly IOverlay _overlay;
        private readonly IWindowAttacher _windowAttacher;
        private ConcurrentDictionary<String, CycleMacro> macros;
        private MacroGallery macroGallery;
        private MacroAddControl macroAddControl;
        private UserControl ownParentWindow;
        private UserControl macroCreateWindow;

        public MacroManager(IOverlay overlay, IWindowAttacher windowAttacher)
        {
            _overlay = overlay;
            _windowAttacher = windowAttacher;
            macros = new ConcurrentDictionary<String, CycleMacro>();
            macroGallery = new MacroGallery();
            macroGallery.initialize();

            macroAddControl = new MacroAddControl();

            ownParentWindow = _overlay.AddWindowToGrid(macroGallery, "Macros", false);
            macroCreateWindow = _overlay.AddWindowToGrid(macroAddControl, "Create new macro", false);

            _overlay.AddItemMenuToMainMenu("Macros",
                                        new Image
                                        {
                                            Source = new BitmapImage(new Uri("pack://application:,,,/Resources/macroMenuIcon.png"))
                                        },
                                        macroMenu_Click);

            PersistenceContainer[] savedMacros = PersistenceUnitThread.persistenceUnit.loadContainersByType(typeof(CycleMacro).Name);
            foreach (PersistenceContainer currentPersistenceContainer in savedMacros)
            {
                CycleMacro newMacro = new CycleMacro();
                newMacro.UpdateCycleMacroByPersistenceContainer(currentPersistenceContainer);
                macros.GetOrAdd(newMacro.Name, newMacro);
            }
        }

        public void macroMenu_Click(object sender, RoutedEventArgs e)
        {
            showMacroMenu();
        }

        public void showCreateMacroMenu()
        {
            macroCreateWindow.Dispatcher.Invoke((Action)(() =>
            {
                macroCreateWindow.Visibility = Visibility.Visible;
            }));
        }

        public void hideCreateMacroMenu()
        {
            macroCreateWindow.Dispatcher.Invoke((Action)(() =>
            {
                macroCreateWindow.Visibility = Visibility.Hidden;
            }));
        }

        public void showMacroMenu()
        {
            ownParentWindow.Dispatcher.Invoke((Action)(() =>
            {
                ownParentWindow.Visibility = Visibility.Visible;
            }));
        }

        public void hideMacroMenu()
        {
            ownParentWindow.Dispatcher.Invoke((Action)(() =>
            {
                ownParentWindow.Visibility = Visibility.Hidden;
            }));
        }

        public void addMacro(CycleMacro macroToAdd)
        {
            macroToAdd.Pause();
            macros.GetOrAdd(macroToAdd.Name, macroToAdd);
            macroToAdd.Persist();
        }

        public void update()
        {
            int tempCount = 0;
            ObservableCollection<MacroItemModel> macroItemModels = new ObservableCollection<MacroItemModel>();
            foreach (KeyValuePair<String, CycleMacro> currentMacroKeyValuePair in macros)
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
            while (!macros.TryRemove(macro.Name, out deletedMacro)) { }

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
            while (!macros.TryRemove(name, out deletedMacro)) { }

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
            if (macros.ContainsKey(name))
            {
                CycleMacro receivedMacro;

                while (!macros.TryGetValue(name, out receivedMacro)) { }
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
            return macros.ToArray();
        }
    }


}
