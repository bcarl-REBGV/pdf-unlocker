using System.ComponentModel;
using System.Runtime.CompilerServices;
using PdfUnlockerGui.Annotations;

namespace PdfUnlockerGui
{
    internal class DataBindingObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Similar to Reactive's SetAndRaiseIfChanged function. Updates the given property and calls OnPropertyChanged
        /// to update UI.
        /// </summary>
        /// <param name="objectToUpdate">A ref to the backing field on which to set the value.</param>
        /// <param name="value">The value to be set.</param>
        /// <param name="memberName" required="false">Name of the property to be passed to the UI. Implicitly passed by compiler.</param>
        /// <typeparam name="T">Type of the backing field and value to set.</typeparam>
        protected void RaiseAndSet<T>(ref T objectToUpdate, T value, [CallerMemberName] string memberName = "")
        {
            objectToUpdate = value;
            OnPropertyChanged(memberName);
        }
    }
}