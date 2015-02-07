using BusFinderUniversal.Model;
using BusFinderUniversal.ViewModel;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Windows.Devices.Geolocation;

namespace BusFinderUniversal
{

	public class TreeNode
	{

		private BusNode currentNode { get; set; }
		private int numberOfChange { get; set; }
		private double weight { get; set; }
		private TreeNode parent { get; set; }
		private double previousDistance { get; set; }

		public TreeNode getParent()
		{
			return parent;
		}

		public double getPreviousDistance()
		{
			return previousDistance;
		}

		public void setPreviousDistance(double previousDistance)
		{
			this.previousDistance = previousDistance;
		}

		public void setParent(TreeNode parent)
		{
			this.parent = parent;
		}

		public TreeNode(BusNode node, int numberOfChange, double previousDistance)
		{
			this.currentNode = node;
			this.numberOfChange = numberOfChange;
			this.previousDistance = previousDistance;
			weight = numberOfChange * 20 + previousDistance;

		}

		public TreeNode(BusNode currentNode, TreeNode parent)
		{
			this.currentNode = currentNode;
			this.parent = parent;
		}

		public BusNode getCurrentNode()
		{
			return currentNode;
		}

		public void setCurrentNode(BusNode currentNode)
		{
			this.currentNode = currentNode;
		}

		public int getNumberOfChange()
		{
			return numberOfChange;
		}

		public void setNumberOfChange(int numberOfChange)
		{
			this.numberOfChange = numberOfChange;
		}

		public double getWeight()
		{
			return weight;
		}

		public void setWeight(double weight)
		{
			this.weight = weight;
		}

	}

	public class AStarAlgo
	{

		private List<TreeNode> OPEN = new List<TreeNode>();             //Tap cac nut con duoc sinh ra nhung chua xet
		private List<TreeNode> offspring = new List<TreeNode>();        //Tap cac trang thai con
		public List<BusNode> answer = new List<BusNode>();            //Loi giai bai toan
		private double fmin;                       //f nho nhat trong cac nut cua OPEN
		private int minIndex;                   //chi so cua nut co f nho nhat trong OPEN
		//private boolean isOver;
		private string startPlace;
		private string endPlace;
		BusStop startStop = null;
		BusStop endStop = null;
		List<BusItem> arrLine;
		List<BusStop> arrayStop;
		int count;
		private static int MAX_SIZE = 8000;

		public string getStartPlace()
		{
			return startPlace;
		}

		public void setStartPlace(string startPlace)
		{
			this.startPlace = startPlace;
		}

		public string getEndPlace()
		{
			return endPlace;
		}

		public void setEndPlace(string endPlace)
		{
			this.endPlace = endPlace;
		}

		public AStarAlgo()
		{
		}

		public AStarAlgo(string startPlace, string endPlace, List<BusStop> arrStop)
		{
			this.startPlace = startPlace;
			this.endPlace = endPlace;
			this.arrayStop = arrStop;
			count = 0;
		}

		public AStarAlgo(BusStop startStop, BusStop endStop)
		{
			this.startStop = startStop;
			this.endStop = endStop;
			this.arrayStop = new List<BusStop>(ServiceLocator.Current.GetInstance<ListBusViewModel>().Buses.Stops);
			count = 0;
		}

		public void saveAnswer(TreeNode n)
		{
			if (n.getParent() != null)
			{
				saveAnswer(n.getParent());
				answer.Add(n.getCurrentNode());
			}
			else
			{
				answer.Add(n.getCurrentNode());
			}
		}


		/*
		 * OPEN is the List which include BusNode not been traverse
		 * answer is the solution 
		 */
		public void algorithm()
		{

			answer.Clear();
			if (startStop == null)
			{
				startStop = searchArrayStop(startPlace);
			}
			if (endStop == null)
			{
				endStop = searchArrayStop(endPlace);
			}
			if (startStop == null || endStop == null)
			{
				return;
			}
			else
			{
				arrLine = new List<BusItem>();
				List<BusNode> tempArrayNode = startStop.arrayNode;

				for (int i = 0; i < endStop.arrayNode.Count; i++)
				{
					arrLine.Add(endStop.arrayNode[i].busItem);
				}


				for (int i = 0; i < tempArrayNode.Count; i++)
				{
					TreeNode newTreeNode = new TreeNode(tempArrayNode[i], 0, 0);
					newTreeNode.setWeight(heuristicFuntion(startStop, endStop));
					newTreeNode.setParent(null);
					OPEN.Add(newTreeNode);

					int index = checkLine(newTreeNode);
					if (index >= 0)
					{
						TreeNode endTreeNode = new TreeNode(endStop.arrayNode[index], newTreeNode);

						saveAnswer(endTreeNode);
						if (CheckValidAnswer())
						{
							OPEN.Clear();
							return;
						}
						else
						{
							answer.Clear();
							OPEN.Clear();
							return;
						}
					}
				}

				while (true)
				{
					if (!OPEN.Any())
					{
						OPEN.Clear();
						if (offspring != null)
						{
							offspring.Clear();
						}
						break;
					}

					//find the min index and the minimum weight of OPEN's BusNode
					minIndex = 0;
					fmin = OPEN[minIndex].getWeight();
					int openSize = OPEN.Count;
					for (int i = 0; i < openSize; i++)
					{
						if (OPEN[i].getWeight() < fmin)
						{
							minIndex = i;
							fmin = OPEN[i].getWeight();
						}
					}

					TreeNode newTreeNode = OPEN[minIndex];
					OPEN.Remove(newTreeNode);
					count++;
					//if you reach end place, save the answer and return
					if (newTreeNode.getCurrentNode().busStop.Code == endStop.Code)
						//|| MyUtil.ConvertVN(newTreeNode.getCurrentNode().busStop.Name.ToLower().Replace(" ", "")) == MyUtil.ConvertVN(endStop.Name.ToLower().Replace(" ", "")))
					{
						//System.out.println("reach end place");
						saveAnswer(newTreeNode);
						if (CheckValidAnswer())
						{
							offspring.Clear();
							return;
						}
						else
						{
							answer.Clear();
							offspring.Clear();
							return;
						}
					}
					if (count == MAX_SIZE)
					{
						answer = null;
						return;
					}
					//get offspring of newTreeNode
					offspring = generateOffspring(newTreeNode);

					if (offspring != null)
					if (offspring.Count > 0)
					{
						int index2 = checkLine(offspring[0]);
						if (index2 >= 0)
						{
							TreeNode endTreeNode = new TreeNode(endStop.arrayNode[index2], offspring[0]);

							saveAnswer(endTreeNode);
							if (CheckValidAnswer())
							{
								offspring.Clear();
								return;
							}
							else 
							{
								answer.Clear();
								offspring.Clear();
								return;
							}
							

						}
						int offspringSize = offspring.Count;
						//add offspring into OPEN
						for (int i = 0; i < offspringSize; i++)
						{
							OPEN.Insert(0, offspring[i]);
						}
					}
				}
			}
		}

		//get OffSpring of a treenode
		public List<TreeNode> generateOffspring(TreeNode n)
		{
			List<BusNode> newArrayBusNode;
			BusStop newStop, currentStop;
			currentStop = n.getCurrentNode().busStop;
			List<TreeNode> newList = new List<TreeNode>();

			if (n.getCurrentNode().nextNode != null)
			{
				newStop = n.getCurrentNode().nextNode.busStop;
				newArrayBusNode = newStop.arrayNode;
			}
			else
			{
				return null;
			}
			int newSize = newArrayBusNode.Count;
			for (int i = 0; i < newSize; i++)
			{
				TreeNode newTreeNode;

				newTreeNode = new TreeNode(newArrayBusNode[i], n);

				if ((newTreeNode.getCurrentNode().busCode == n.getCurrentNode().busCode) 
					&& (newTreeNode.getCurrentNode().busStop.Code == n.getCurrentNode().busStop.Code))
				{
					continue;
				}
				else if (checkLine(newTreeNode) >= 0)
				{
					newTreeNode.setParent(n);
					newList.Insert(0, newTreeNode);
					return newList;
				}


				else if (newTreeNode.getCurrentNode().busCode != n.getCurrentNode().busCode)
				{
					newTreeNode.setNumberOfChange(n.getNumberOfChange() + 1);
					newTreeNode.setPreviousDistance(n.getPreviousDistance() + MyUtil.DistanceInKiloMetres(MyUtil.textToGeoList(currentStop.geo).First(), MyUtil.textToGeoList(newStop.geo).First()));
					newTreeNode.setWeight(newTreeNode.getNumberOfChange() * 6.25 + newTreeNode.getPreviousDistance() + heuristicFuntion(newStop, endStop));
					newTreeNode.setParent(n);
					newList.Add(newTreeNode);
				}
				//else if (!(newTreeNode.getCurrentNode().busCode == n.getCurrentNode().busCode)
				//		|| //(newTreeNode.getCurrentNode().getLine().getDirection() != n.getCurrentNode().getLine().getDirection() && 
				//		newTreeNode.getCurrentNode().busCode == n.getCurrentNode().busCode)
				//{ //) {
				//	newTreeNode.setNumberOfChange(n.getNumberOfChange() + 1);
				//	newTreeNode.setPreviousDistance(n.getPreviousDistance() + MyUtil.DistanceInKiloMetres(currentStop.geo, newStop.geo));
				//	newTreeNode.setWeight(newTreeNode.getNumberOfChange() * 6.25 + newTreeNode.getPreviousDistance() + heuristicFuntion(newStop, endStop));
				//	newTreeNode.setParent(n);
				//	newList.Add(newTreeNode);
				//}


				else
				{
					newTreeNode.setNumberOfChange(n.getNumberOfChange());
					newTreeNode.setPreviousDistance(n.getPreviousDistance() + MyUtil.DistanceInKiloMetres(MyUtil.textToGeoList(currentStop.geo).First(), MyUtil.textToGeoList(newStop.geo).First()));
					newTreeNode.setWeight(newTreeNode.getNumberOfChange() * 6.25 + newTreeNode.getPreviousDistance() + heuristicFuntion(newStop, endStop));
					newTreeNode.setParent(n);
					newList.Add(newTreeNode);
				}

			}
			return newList;
		}

		public bool CheckValidAnswer()
		{
			for (int i = 0; i < answer.Count - 1; i++)
			{
				BusNode nxtNode = answer[i].nextNode;
				if (nxtNode == null)
				{
					return false;
				}

				if (nxtNode.busStop.Code == answer.Last().busStop.Code)
				{
					break;
				}
				while (nxtNode.busStop.Code != answer[i + 1].busStop.Code)
				{
					nxtNode = nxtNode.nextNode;
					if (nxtNode == null)
					{
						return false;
					}
				}
			}
			return true;
		}

		public double heuristicFuntion(BusStop currentStop, BusStop endStop)
		{
			Geopoint geo1 = MyUtil.textToGeoList(currentStop.geo).First();
			Geopoint geo2 = MyUtil.textToGeoList(endStop.geo).First();
			return MyUtil.DistanceInKiloMetres(geo1, geo2);
		}

		public int checkLine(TreeNode n)
		{
			int result = -1;
			for (int i = 0; i < arrLine.Count; i++)
			{
				if (arrLine[i].Code == n.getCurrentNode().busCode)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		public int getCount()
		{
			return count;
		}

		public BusStop searchArrayStop(string name)
		{
			for (int i = 0; i < arrayStop.Count; i++)
			{
				if (arrayStop[i].Code == name)
				{
					return arrayStop[i];
				}
			}
			return null;
		}


	}

	//public class AStarAlgo
	//{

	//	private List<TreeNode> OPEN = new List<TreeNode>();             //Tap cac nut con duoc sinh ra nhung chua xet
	//	private List<TreeNode> offspring = new List<TreeNode>();        //Tap cac trang thai con
	//	public List<BusNode> answer = new List<BusNode>();            //Loi giai bai toan
	//	private double fmin;                       //f nho nhat trong cac nut cua OPEN
	//	private int minIndex;                   //chi so cua nut co f nho nhat trong OPEN
	//	//private boolean isOver;
	//	private string startPlace;
	//	private string endPlace;
	//	BusStop startStop = null;
	//	BusStop endStop = null;
	//	List<BusItem> arrLine;
	//	List<BusStop> arrayStop;
	//	int count;
	//	private static int MAX_SIZE = 8000;

	//	public string getStartPlace()
	//	{
	//		return startPlace;
	//	}

	//	public void setStartPlace(string startPlace)
	//	{
	//		this.startPlace = startPlace;
	//	}

	//	public string getEndPlace()
	//	{
	//		return endPlace;
	//	}

	//	public void setEndPlace(string endPlace)
	//	{
	//		this.endPlace = endPlace;
	//	}

	//	public AStarAlgo()
	//	{
	//	}

	//	public AStarAlgo(string startPlace, string endPlace, List<BusStop> arrStop)
	//	{
	//		this.startPlace = startPlace;
	//		this.endPlace = endPlace;
	//		this.arrayStop = arrStop;
	//		count = 0;
	//	}

	//	public AStarAlgo(BusStop startStop, BusStop endStop)
	//	{
	//		this.startStop = startStop;
	//		this.endStop = endStop;
	//		this.arrayStop = new List<BusStop>(ServiceLocator.Current.GetInstance<ListBusViewModel>().Buses.Stops);
	//		count = 0;
	//	}

	//	public void saveAnswer(TreeNode n)
	//	{
	//		if (n.getParent() != null)
	//		{
	//			saveAnswer(n.getParent());
	//			answer.Add(n.getCurrentNode());
	//		}
	//		else
	//		{
	//			answer.Add(n.getCurrentNode());
	//		}
	//	}

	//	/*
	//	 * OPEN is the List which include BusNode not been traverse
	//	 * answer is the solution 
	//	 */
	//	public void algorithm()
	//	{

	//		answer.Clear();
	//		if (startStop == null)
	//		{
	//			startStop = searchArrayStop(startPlace);
	//		}
	//		if (endStop == null)
	//		{
	//			endStop = searchArrayStop(endPlace);
	//		}
	//		if (startStop == null || endStop == null)
	//		{
	//			return;
	//		}
	//		else
	//		{
	//			arrLine = new List<BusItem>();
	//			List<BusNode> tempArrayNode = startStop.arrayNode;
	//			for (int i = 0; i < tempArrayNode.Count; i++)
	//			{
	//				TreeNode newTreeNode = new TreeNode(tempArrayNode[i], 0, 0);
	//				newTreeNode.setWeight(heuristicFuntion(startStop, endStop));
	//				newTreeNode.setParent(null);
	//				OPEN.Add(newTreeNode);
	//			}

	//			for (int i = 0; i < endStop.arrayNode.Count; i++)
	//			{
	//				//arrLine.Add(endStop.arrayNode[i].getLine());
	//				BusItem bus = ServiceLocator.Current.GetInstance<ListBusViewModel>().FindBusItemByCode(endStop.arrayNode[i].busCode);
	//				arrLine.Add(bus);
	//			}

	//			while (true)
	//			{
	//				if (!OPEN.Any())
	//				{
	//					//System.out.println("12345");
	//					OPEN.Clear();
	//					if (offspring != null)
	//					{
	//						offspring.Clear();
	//					}
	//					break;
	//				}

	//				//find the min index and the minimum weight of OPEN's BusNode
	//				minIndex = 0;
	//				fmin = OPEN[minIndex].getWeight();
	//				int openSize = OPEN.Count;
	//				for (int i = 0; i < openSize; i++)
	//				{
	//					if (OPEN[i].getWeight() < fmin)
	//					{
	//						minIndex = i;
	//						fmin = OPEN[i].getWeight();
	//					}
	//				}


	//				TreeNode newTreeNode = OPEN[minIndex];
	//				OPEN.Remove(newTreeNode);
	//				count++;
	//				//if you reach end place, save the answer and return
	//				if (newTreeNode.getCurrentNode().busStop.Code == endStop.Code)
	//				{
	//					//System.out.println("reach end place");
	//					saveAnswer(newTreeNode);
	//					offspring.Clear();
	//					return;
	//				}
	//				if (count == MAX_SIZE)
	//				{
	//					answer = null;
	//					return;
	//				}
	//				//get offspring of newTreeNode
	//				offspring = generateOffspring(newTreeNode);

	//				if (offspring != null)
	//				{
	//					int index = checkLine(offspring[0]);
	//					if (index >= 0)
	//					{
	//						TreeNode endTreeNode = new TreeNode(endStop.arrayNode[index], offspring[0]);
	//						//System.out.println("reach end place (1)");
	//						saveAnswer(endTreeNode);
	//						offspring.Clear();
	//						return;
	//					}
	//					int offspringSize = offspring.Count;
	//					//add offspring into OPEN
	//					for (int i = 0; i < offspringSize; i++)
	//					{
	//						OPEN.Insert(0, offspring[i]);
	//					}

	//				}


	//			}
	//		}
	//	}

	//	//get OffSpring of a treenode
	//	public List<TreeNode> generateOffspring(TreeNode n)
	//	{

	//		List<BusNode> newArrayBusNode;
	//		BusStop newStop, currentStop;
	//		currentStop = searchArrayStop(n.getCurrentNode().busStop.Code);
	//		List<TreeNode> newList = new List<TreeNode>();

	//		if (n.getCurrentNode().nextNode != null)
	//		{
	//			string stop_name = n.getCurrentNode().nextNode.busStop.Code;
	//			newStop = searchArrayStop(stop_name);
	//			newArrayBusNode = newStop.arrayNode;

	//		}
	//		else
	//		{
	//			return null;
	//		}
	//		int newSize = newArrayBusNode.Count;
	//		for (int i = 0; i < newSize; i++)
	//		{
	//			TreeNode newTreeNode;
	//			try
	//			{
	//				newTreeNode = new TreeNode(newArrayBusNode[i], n);

	//				if ((newTreeNode.getCurrentNode().busCode == n.getCurrentNode().busCode) && (newTreeNode.getCurrentNode().busStop.Code == n.getCurrentNode().busStop.Code))
	//				{
	//					continue;
	//				}
	//				else if (checkLine(newTreeNode) >= 0)
	//				{
	//					newTreeNode.setParent(n);
	//					newList.Insert(0, newTreeNode);
	//					return newList;
	//				}
	//				else if (!(newTreeNode.getCurrentNode().busCode == n.getCurrentNode().busCode)
	//						|| //(newTreeNode.getCurrentNode().getLine().getDirection() != n.getCurrentNode().getLine().getDirection() && 
	//						newTreeNode.getCurrentNode().busCode == n.getCurrentNode().busCode)
	//				{ //) {
	//					newTreeNode.setNumberOfChange(n.getNumberOfChange() + 1);
	//					newTreeNode.setPreviousDistance(n.getPreviousDistance() + MyUtil.DistanceInKiloMetres(currentStop.geo, newStop.geo));
	//					newTreeNode.setWeight(newTreeNode.getNumberOfChange() * 6.25 + newTreeNode.getPreviousDistance() + heuristicFuntion(newStop, endStop));
	//					newTreeNode.setParent(n);
	//					newList.Add(newTreeNode);
	//				}
	//				else
	//				{
	//					newTreeNode.setNumberOfChange(n.getNumberOfChange());
	//					newTreeNode.setPreviousDistance(n.getPreviousDistance()
	//							+ MyUtil.DistanceInKiloMetres(newStop.geo, currentStop.geo));
	//					newTreeNode.setWeight(newTreeNode.getNumberOfChange() * 6.25 + newTreeNode.getPreviousDistance() + heuristicFuntion(newStop, endStop));

	//					newTreeNode.setParent(n);
	//					newList.Add(newTreeNode);
	//				}
	//			}
	//			catch (Exception exc)
	//			{
	//				string message = exc.Message;
	//			}
	//		}
	//		return newList;
	//	}

	//	public double heuristicFuntion(BusStop currentStop, BusStop endStop)
	//	{
	//		Geopoint geo1 = currentStop.geo;
	//		Geopoint geo2 = endStop.geo;
	//		return MyUtil.DistanceInKiloMetres(geo1, geo2);
	//	}

	//	public int checkLine(TreeNode n)
	//	{
	//		int result = -1;
	//		for (int i = 0; i < arrLine.Count; i++)
	//		{
	//			if (arrLine[i].Code == n.getCurrentNode().busCode)
	//			{
	//				result = i;
	//				break;
	//			}
	//		}
	//		return result;
	//	}

	//	public int getCount()
	//	{
	//		return count;
	//	}

	//	public BusStop searchArrayStop(string name)
	//	{
	//		for (int i = 0; i < arrayStop.Count; i++)
	//		{
	//			if (arrayStop[i].Code == name)
	//			{
	//				return arrayStop[i];
	//			}
	//		}
	//		return null;
	//	}


	//}

	public class ResultObject
	{
		private BusStop origin;
		private BusStop destination;
		private string busCode;

		public ResultObject(BusStop origin, BusStop destination, string busCode)
		{
			this.origin = origin;
			this.destination = destination;
			this.busCode = busCode;
		}

		public BusStop getOrigin()
		{
			return origin;
		}

		public void setOrigin(BusStop origin)
		{
			this.origin = origin;
		}

		public BusStop getDestination()
		{
			return destination;
		}

		public void setDestination(BusStop destination)
		{
			this.destination = destination;
		}

		public string getLine()
		{
			return busCode;
		}

		public void setLine(string busCode)
		{
			this.busCode = busCode;
		}

	}

	public class ResultSearchObject
	{

		private string startEndDeclare;
		private string detailInfo;

		public ResultSearchObject(string startEndDeclare, string detailInfo)
		{
			this.startEndDeclare = startEndDeclare;
			this.detailInfo = detailInfo;
		}

		public string getStartEndDeclare()
		{
			return startEndDeclare;
		}

		public void setStartEndDeclare(string startEndDeclare)
		{
			this.startEndDeclare = startEndDeclare;
		}

		public string getDetailInfo()
		{
			return detailInfo;
		}

		public void setDetailInfo(string detailInfo)
		{
			this.detailInfo = detailInfo;
		}

	}
}
