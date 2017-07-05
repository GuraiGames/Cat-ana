using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    List<MyEvent> events = new List<MyEvent>();
    List<QueueEvent> queue_events = new List<QueueEvent>();

    public delegate void MyEventHandler(MyEvent ev);

    public MyEvent CreateEvent(string name, MyEventHandler listener)
    {
        MyEvent ret = null;

        ret = new MyEvent(name, listener, DeleteEvent);
        events.Add(ret);

        return ret;
    }

    private void Update()
    {
        if(events.Count > 150)
        {
            events.RemoveAt(0);
        }
    }

    public void CreateQueueEvent(string name, MyEventHandler listener)
    {
        QueueEvent ev = new QueueEvent(name, listener, QueueEventFinished, DeleteEvent);
        queue_events.Add(ev);
    }

    public MyEvent GetEvent(string eventName)
    {
        MyEvent ret = null;

        for (int i = 0; i < events.Count; i++)
        {
            if (!events[i].IsDisabled() && events[i].GetEventName() == eventName)
            {
                ret = events[i];
                break;
            }
        }

        return ret;
    }

    public void DeleteEvent(string eventName)
    {
        MyEvent ev = GetEvent(eventName);

        if (ev != null)
        {
            if(!events.Remove(ev))
                queue_events.Remove((QueueEvent)ev);
        }
    }

    public void TriggerEvent(string eventName)
    {
        MyEvent ev = GetEvent(eventName);

        if(ev != null)
            ev.TriggerEvent();
    }

    public void QueueEventFinished(MyEvent ev)
    {
        if(queue_events[0] == ev)
            queue_events.RemoveAt(0);

        if (queue_events.Count > 0)
            queue_events[0].TriggerEvent();
    }

    public void DeleteEvent(MyEvent ev)
    {
        events.Remove(ev);
        ev = null;
    }

    public class MyEvent
    {
        public MyEvent(string name, MyEventHandler listener, MyEventHandler remove_listener)
        {
            _name = name;
            _listener = listener;
        }

        public void TriggerEvent()
        {
            if (_listener != null)
                _listener.Invoke(this);
        }

        public void AddString(int index, string value)
        {
            ExpandListToIndex(index);

            _data[index] = value;
        }

        public void AddInt(int index, int value)
        {
            ExpandListToIndex(index);

            _data[index] = value.ToString();
        }

        public void AddBool(int index, bool value)
        {
            ExpandListToIndex(index);

            _data[index] = value.ToString();
        }

        public string GetString(int index)
        {
            return _data[index];
        }

        public int GetInt(int index)
        {
            return int.Parse(_data[index]);
        }

        public bool GetBool(int index)
        {
            if (_data[index] == "true")
                return true;
            else
                return false;
        }

        public void Delete()
        {
            _delete_listener.Invoke(this);
        }

        public void Disable()
        {
            disabled = true;
        }

        public void Enable()
        {
            disabled = false;
        }

        public bool IsDisabled()
        {
            return disabled;
        }

        public string GetEventName() { return _name; }

        private void ExpandListToIndex(int index)
        {
            if (index > 1000)
                return;
            
            while(_data.Count <= index)
                _data.Add("");
        }

        private string _name = "";
        private MyEventHandler _listener = null;
        private MyEventHandler _delete_listener = null;
        List<string> _data = new List<string>();
        bool disabled = false;
    }

    public class QueueEvent : MyEvent
    {
        public QueueEvent(string name, MyEventHandler listener, MyEventHandler finished_listener, MyEventHandler delete_listener) : base(name, listener, delete_listener) {}
        
        public void SendEventFinished()
        {
            listener_finished.Invoke(this);
        }

        private MyEventHandler listener_finished = null;
    }
}
