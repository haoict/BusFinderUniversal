using BusFinderUniversal.Model;
using BusFinderUniversal.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BusFinderUniversal.ViewModel
{
	public class FindRouteResultViewModel : ViewModelBase
	{
		ListBusViewModel currentListInstance = ServiceLocator.Current.GetInstance<ListBusViewModel>();
		private FindRouteResultModel _result;
		private string _from;
		private string _to;

		public string From
		{
			get
			{
				return _from;
			}
			set
			{
				Set("From", ref _from, value);
			}
		}
		public string To
		{
			get
			{
				return _to;
			}
			set
			{
				Set("To", ref _to, value);
			}
		}
		public FindRouteResultModel Result
		{
			get
			{
				return _result;
			}
			set
			{
				Set("Result", ref _result, value);
			}
		}
		


		public RelayCommand<ItemClickEventArgs> ResultSelectedCommand { get; private set; }

		public FindRouteResultViewModel()
		{
			Messenger.Default.Register<FindRouteResultModel>(this, ProcessFindResut);
			ResultSelectedCommand = new RelayCommand<ItemClickEventArgs>((selectedItem) => ResultSelectedHandler(selectedItem));
		}

		private void ResultSelectedHandler(ItemClickEventArgs selectedItem)
		{
			FindRouteResultDetailModel it = selectedItem.ClickedItem as FindRouteResultDetailModel;
			if (!((Frame)Window.Current.Content).Navigate(typeof(FindRouteResultMapView)))
			{
				throw new Exception("NavigationFailedExceptionMessage");
			}
			Messenger.Default.Send(it);
			//MessageDialogHelper.Show(it.Detail);
		}

		private async void ProcessFindResut(FindRouteResultModel answer)
		{
			Result = answer;
			From = Result.From;
			To = Result.To;

			int id = 0;
			foreach (List<BusNode> ans in Result.foundAnswerList)
			{
				string busItemsThrough = "Đi tuyến ";
				foreach (BusNode bn in ans)
				{
					if (!busItemsThrough.Contains(bn.busCode))
					{
						busItemsThrough += bn.busCode;
						busItemsThrough += ", ";
					}
				}
				busItemsThrough = busItemsThrough.Remove(busItemsThrough.Length - 2);

				FindRouteResultDetailModel tmp = new FindRouteResultDetailModel();
				tmp.ResultID = id;
				tmp.ResultName = busItemsThrough;
				tmp.ResultDescription = "Đi xe bus: 6 km, đi bộ: 0.3 km";
				tmp.ResultTimeConsume = "Ước lượng thời gian: 45 phút";
				tmp.Detail += AStarResultToString(ans);
				tmp.ResultNodes = answer.foundAnswerList[id];
				tmp.FromPoint = answer.FromPoint;
				tmp.ToPoint = answer.ToPoint;
				Result.ResultDetail.Add(tmp);
				id++;
			}

			if (Result.FromPoint != null)
				From = await MyUtil.GetPlaceNameByLocation(Result.FromPoint);
			if (Result.ToPoint != null)
				To = await MyUtil.GetPlaceNameByLocation(Result.ToPoint);
		}

		private List<ResultObject> convertToResultObject(List<BusNode> arrNodes)
		{
			List<ResultObject> result = new List<ResultObject>();
			BusNode startNode;
			startNode = arrNodes[0];
			int foundSize = arrNodes.Count;
			for (int k = 1; k < foundSize; k++)
			{
				BusNode current = arrNodes[k];
				if (current.busCode == startNode.busCode && k < foundSize - 1)
				{
					continue;
				}
				else if (current.busCode == startNode.busCode && k == foundSize - 1)
				{
					ResultObject ro = new ResultObject(startNode.busStop, arrNodes[k].busStop, startNode.busCode);
					result.Add(ro);
				}

				else
				{
					ResultObject ro = new ResultObject(startNode.busStop, arrNodes[k].busStop, startNode.busCode);
					result.Add(ro);
					startNode = current;
				}
			}
			return result;
		}
		private string AStarResultToString(List<BusNode> foundAnswer)
		{
			if (foundAnswer == null)
			{
				return "";
			}

			List<ResultObject> searchResultElement = new List<ResultObject>();
			searchResultElement = convertToResultObject(foundAnswer);
			StringBuilder detail = new StringBuilder();
			String de = "";
			for (int index = 0; index < searchResultElement.Count; index++)
			{
				ResultObject ro = searchResultElement[index];
				if (index == 0)
				{
					detail.Append("- Di chuyển đến " + ro.getOrigin().Name + ", bắt xe Bus " + ro.getLine()
						+ "(" + currentListInstance.FindBusItemByCode(ro.getLine()).Name + ") tới "
						+ ro.getDestination().Name + "\n");
				}
				else
				{
					detail.Append("- Từ " + ro.getOrigin().Name + ", bắt xe Bus " + ro.getLine()
						+ "(" + currentListInstance.FindBusItemByCode(ro.getLine()).Name + ") tới "
						+ ro.getDestination().Name + "\n");
				}
				de = detail.ToString();
			}

			ResultSearchObject rso = new ResultSearchObject(searchResultElement[0].getOrigin() + " - "
					+ searchResultElement[searchResultElement.Count - 1].getDestination(), detail.ToString());

			searchResultElement.Clear();
			return de;
		}
	}
}
