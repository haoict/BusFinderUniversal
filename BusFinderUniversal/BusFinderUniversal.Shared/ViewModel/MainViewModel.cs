using BusFinderUniversal.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BusFinderUniversal.ViewModel
{
	public class MainViewModel : ViewModelBase
	{

		/// <summary>
		/// Gets or sets the person.
		/// </summary>
		/// <value>
		/// The person.
		/// </value>
		public Person Person
		{
			get
			{
				return _person;
			}
			set
			{
				Set("Person", ref _person, value);
			}
		}

		/// <summary>
		/// The person
		/// </summary>
		private Person _person;

		/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		public MainViewModel()
		{
			if (IsInDesignMode)
			{
				// Code runs in Blend --> create design time data.
				_person = new Person { Name = "Sara", Age = 30 };
			}
			else
			{
				// Code runs "for real"
				_person = new Person { Name = "Mary", Age = 35 };
			}

			PropertyChanged += MainViewModel_PropertyChanged;
			ShowMessageCommand = new RelayCommand(ShowMessage);
			IncreaseAgeCommand = new RelayCommand(IncreaseAge);
		}

		/// <summary>
		/// Gets the show message command.
		/// </summary>
		/// <value>
		/// The show message command.
		/// </value>
		public ICommand ShowMessageCommand { get; private set; }
		public ICommand IncreaseAgeCommand { get; private set; }

		/// <summary>
		/// Unregisters this instance from the Messenger class.
		/// <para>To cleanup additional resources, override this method, clean
		/// up and then call base.Cleanup().</para>
		/// </summary>
		public override void Cleanup()
		{
			base.Cleanup();
			PropertyChanged -= MainViewModel_PropertyChanged;
		}

		/// <summary>
		/// Handles the PropertyChanged event of the MainViewModel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
		private void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Person")
			{
				
			}
		}

		/// <summary>
		/// Shows the message.
		/// </summary>
		private void ShowMessage()
		{
			Messenger.Default.Send("This is a message.");
		}

		private void IncreaseAge()
		{
			_person.Age += 5;
		}

	}
}
