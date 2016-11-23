using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BDMultiTool.Persistence;

namespace BDMultiTool.Macros {
    public class CycleMacro {
        private readonly LinkedList<Keys> _keys;
        public bool Paused { get; set; }
        public long Interval { get; set;}
        public long Lifetime { get; set; }
        public string Name { get; set; }
        private bool _initialized;
        private readonly Stopwatch _stopWatchTotalTime;
        private readonly Stopwatch _stopWatch;

        public CycleMacro(params Keys[] keys) {
            _keys = new LinkedList<Keys>(keys);
            Name = "N/A";
            _stopWatch = new Stopwatch();
            _stopWatchTotalTime = new Stopwatch();
            _initialized = false;
            Interval = 2000;
            Lifetime = -1;
            Paused = true;
        }

        public CycleMacro() {
            _keys = new LinkedList<Keys>();
            Name = "N/A";
            _stopWatch = new Stopwatch();
            _stopWatchTotalTime = new Stopwatch();
            _initialized = false;
            Interval = 2000;
            Lifetime = -1;
            Paused = true;
        }

        public void AddKey(Keys key) {
            _keys.AddLast(key);
        }

        public void Start() {
            _stopWatch.Start();
            if(!_initialized) {
                _initialized = true;
                if(Lifetime > 0) {
                    _stopWatchTotalTime.Start();
                }
            }
            Paused = false;
        }

        public void Pause() {
            _stopWatch.Stop();
            _stopWatchTotalTime.Stop();
            Paused = true;
        }

        public void Resume() {
            _stopWatch.Start();
            _stopWatchTotalTime.Start();
            Paused = false;
        }

        public string GetKeyString() {
            var stringBuilder = new StringBuilder();
            var firstItem = true;
            foreach(var currentKey in _keys) {
                if(!firstItem) {
                    stringBuilder.Append(", ");
                } else {
                    firstItem = false;
                }
                stringBuilder.Append(currentKey);
            }

            return stringBuilder.ToString();
        }

        public TimeSpan GetRemainingCoolDown() {
            return TimeSpan.FromMilliseconds(Interval - _stopWatch.ElapsedMilliseconds);
        }

        public string GetRemainingCoolDownFormatted() {
            return GetFormattedTimeSpan(GetRemainingCoolDown());
        }

        public TimeSpan GetRemainingLifeTime()
        {
            if(Lifetime > 0) {
                return TimeSpan.FromMilliseconds(Lifetime - _stopWatchTotalTime.ElapsedMilliseconds);
            }
            return new TimeSpan();
        }

        public string GetRemainingLifeTimeFormatted()
        {
            if(Lifetime < 0) {
                return "";
            }
            return GetFormattedTimeSpan(GetRemainingLifeTime());
        }

        private string GetFormattedTimeSpan(TimeSpan timeSpan) {
            var stringBuilder = new StringBuilder();
            if (timeSpan.TotalMinutes >= 60) {
                stringBuilder.Append(">");
            }
            if(timeSpan.TotalSeconds <= 1) {
                if (timeSpan.Milliseconds < 1000) {
                    stringBuilder.Append(" ");
                }
                if (timeSpan.Milliseconds < 100) {
                    stringBuilder.Append(" ");
                }
                if (timeSpan.Milliseconds < 10) {
                    stringBuilder.Append(" ");
                }
                stringBuilder.Append(timeSpan.Milliseconds);
                stringBuilder.Append(" ms");
            } else {
                if (timeSpan.Minutes < 10) {
                    stringBuilder.Append("0");
                }
                stringBuilder.Append(timeSpan.Minutes);
                stringBuilder.Append(":");
                if (timeSpan.Seconds < 10) {
                    stringBuilder.Append("0");
                }
                stringBuilder.Append(timeSpan.Seconds);
            }

            return stringBuilder.ToString();
        }

        public float GetLifeTimePercentage()
        {
            if (Lifetime > 0) {

                return (_stopWatchTotalTime.ElapsedMilliseconds/(float)Lifetime) * 100f;
            }
            return 100;
        }

        public float GetCoolDownPercentage() {
            return (_stopWatch.ElapsedMilliseconds / (float)Interval) * 100f;
        }

        public bool LifeTimeOver()
        {
            if(Lifetime > 0) {
                return _stopWatchTotalTime.ElapsedMilliseconds >= Lifetime;
            }
            return false;
        }

        public bool IsReady()
        {
            if(_stopWatch.ElapsedMilliseconds >= Interval) {
                return true;
            }
            return false;
        }

        public void Reset() {
            _stopWatch.Reset();
            Paused = true;
        }

        public void ResetAll() {
            _stopWatchTotalTime.Reset();
            Reset();
        }

        public Keys[] GetKeys() {
            return _keys.ToArray();
        }

        public void UpdateCycleMacroByPersistenceContainer(PersistenceContainer temporaryPersistenceContainer) {
            if (temporaryPersistenceContainer != null) {
                Name = temporaryPersistenceContainer.content.Element("name").Value;
                Interval = long.Parse(temporaryPersistenceContainer.content.Element("interval").Value);
                Lifetime = long.Parse(temporaryPersistenceContainer.content.Element("lifetime").Value);
                AddKeysByString(temporaryPersistenceContainer.content.Element("keys").Value);
            }
        }

        public void Persist() {
            PersistenceUnitThread.persistenceUnit.addToPersistenceBuffer(PersistenceUnit.createPersistenceContainer(Name + GetType().Name,
                                                                                                                    GetType().Name,
                                                                                                                    new[]
                                                                                                                    {
                                                                                                                        new[] { "interval", Interval.ToString() },
                                                                                                                        new[] { "lifetime", Lifetime.ToString() },
                                                                                                                        new[] { "name", Name },
                                                                                                                        new[] { "keys", GetStringFromKeys() }
                                                                                                                    }));

        }

        private string GetStringFromKeys() {
            var stringBuilder = new StringBuilder();
            var firstElement = true;

            foreach(var key in _keys) {
                if(!firstElement) {
                    stringBuilder.Append(";");
                } else {
                    firstElement = false;
                }
                stringBuilder.Append((int)key);
            }

            return stringBuilder.ToString();
        }

        private void AddKeysByString(string stringKeys) {
            var separatedKeys = stringKeys.Split(';');

            foreach(var currentKey in separatedKeys) {
                if (currentKey != "" && currentKey.Length > 0) {
                    _keys.AddLast((Keys)int.Parse(currentKey));
                }

            }
        }

        public void DeletePersistence() {
            PersistenceUnitThread.persistenceUnit.deleteByKy(Name + GetType().Name);
        }
    }
}
