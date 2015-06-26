using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ArmaBrowser
{
    internal  class Logger : IObservable<string>
    {
        static Logger _default = new Logger();
        private List<IObserver<string>> _clients = new List<IObserver<string>>();

        public static Logger Default { get { return _default; } }

        private Logger()
        {

        }

        public IDisposable Subscribe(IObserver<string> observer)
        {
            return new Unsubscriber(_clients, observer);
            
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<string>> _observers;
            private IObserver<string> _observer;

            public Unsubscriber(List<IObserver<string>> observers, IObserver<string> observer)
            {
                observers.Add(observer);
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        internal void Push(string s)
        {
            foreach (var observer in _clients)
            {
                observer.OnNext(s);
            }
        }

        internal void PushLine(string s)
        {
            foreach (var observer in _clients)
            {
                observer.OnNext(s + Environment.NewLine);
            }
        }
    }


    internal class TextBoxLoggerApender : IObserver<string>
    {
        TextBox _textBox;

        public TextBoxLoggerApender(TextBox textBox)
        {
            _textBox = textBox;
        }

        // Zusammenfassung:
        //     Benachrichtigt den Beobachter, dass der Anbieter aufgehört hat, Pushbenachrichtigungen
        //     zu senden.
        public void OnCompleted()
        {
            _textBox = null;
        }

        //
        // Zusammenfassung:
        //     Benachrichtigt den Beobachter, dass beim Anbieter ein Fehlerzustand aufgetreten
        //     ist.
        //
        // Parameter:
        //   error:
        //     Ein Objekt, das zusätzliche Informationen zum Fehler bereitstellt.
        public void OnError(Exception error)
        {
             
        }

        //
        // Zusammenfassung:
        //     Stellt neue Daten für den Beobachter bereit.
        //
        // Parameter:
        //   value:
        //     Die aktuellen Benachrichtigungsinformationen.
        public void OnNext(string value)
        {
            if (_textBox != null)
                _textBox.AppendText(value);
        }
    }
}
