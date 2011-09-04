using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GWNorthEngine.Scripting;
using GWNorthEngine.Input;
namespace Robber {
	public class MapEditor {
		public enum MappingState {
			None,
			PlayerStart,
			GuardPosition,
			GuardEntry,
			Treasure,
			WayPoint,
			Dumpster
		};
		//singleton variable
		private static MapEditor instance = new MapEditor();

		#region Class variables
		private MappingState mappingState;
		private const string COMMAND_NONE = "none";
		private const string COMMAND_PLAYER_POSITION = "playerposition";
		private const string COMMAND_GUARD_POSITION = "guardposition";
		private const string COMMAND_GUARD_ENTRY = "guardentry";
		private const string COMMAND_TREASURE = "treasure";
		private const string COMMAND_WAY_POINTS = "waypoint";
		public const string COMMAND_DUMPSTER = "dumpster";
		public static string XML_X = "X";
		public static string XML_Y = "Y";
		public static string XML_R = "R";
		public static string XML_G = "G";
		public static string XML_B = "B";
		public static string XML_TIME = "Time";
		public static string XML_MINUTES = "Minutes";
		public static string XML_WALL_COLOUR = "Walls";
		public static string XML_FLOOR_COLOUR = "Floor";
		public static string XML_HEADER_GUARD = "Guard";
		public static string XML_GUARD_STATE = "State";
		public static string XML_GUARD_DIRECTION = "Direction";
		public static string XML_GUARD_POSITION = "Position";
		
		#endregion Class variables
		
		#region Class properties

		#endregion Class properties

		#region Constructor
		public MapEditor() {
			this.mappingState = MappingState.None;
#if WINDOWS
#if DEBUG
			ScriptManager.getInstance().registerObject(this, "editor");
#endif
#endif
		}
		#endregion Constructor

		#region Support methods
		public static MapEditor getInstance() {
			return instance;
		}

		public void editMapHelp() {
			Console.WriteLine("Format: [Information] - [Command]");
			Console.WriteLine("Turn Mapping off - " + COMMAND_NONE);
			Console.WriteLine("Players starting position - " + COMMAND_PLAYER_POSITION);
			Console.WriteLine("Guard starting position(Should be a waypoint) - " + COMMAND_GUARD_POSITION );
			Console.WriteLine("Guard entry point / Exit point - " + COMMAND_GUARD_ENTRY);
			Console.WriteLine("Treasure - " + COMMAND_TREASURE);
			Console.WriteLine("Guard way points - " + COMMAND_WAY_POINTS);
			Console.WriteLine("Dumpsters - " + COMMAND_DUMPSTER);
		}

		public void editMap(string value) {
			value = value.ToLower();
			MappingState oldState = this.mappingState;
			switch (value) {
				case COMMAND_NONE:
					this.mappingState = MappingState.None;
					break;
				case COMMAND_PLAYER_POSITION:
					this.mappingState = MappingState.PlayerStart;
					break;
				case COMMAND_GUARD_POSITION:
					this.mappingState = MappingState.GuardPosition;
					break;
				case COMMAND_GUARD_ENTRY:
					this.mappingState = MappingState.GuardEntry;
					break;
				case COMMAND_TREASURE:
					this.mappingState = MappingState.Treasure;
					break;
				case COMMAND_WAY_POINTS:
					this.mappingState = MappingState.WayPoint;
					break;
				case COMMAND_DUMPSTER:
					this.mappingState = MappingState.Dumpster;
					break;
				default:
					Console.WriteLine("Failed to recognize your command, try using the editMapHelp()");
					break;
			}
			if (this.mappingState != oldState) {
				ScriptManager.getInstance().log("Changed to edit: " + this.mappingState.ToString());
			}
			Console.WriteLine("Mapping: " + this.mappingState.ToString());
		}

		public void update() {
			if (InputManager.getInstance().wasLeftButtonPressed() && 
				InputManager.getInstance().MouseY >= 0 && InputManager.getInstance().MouseX >= 0) {
				StringBuilder xml = new StringBuilder();
				Point indexPosition = Placement.getIndex(InputManager.getInstance().MousePosition);

				switch (this.mappingState) {
					case MappingState.GuardPosition:
						xml.Append("\t\t<Guard>");
						xml.Append("\n\t\t\t<State></State>");
						xml.Append("\n\t\t\t<Direction></Direction>");
						xml.Append("\n\t\t\t<Position>");
						xml.Append("\n\t\t\t\t<" + XML_X + ">" + indexPosition.X + "</" + XML_X + ">");
						xml.Append("\n\t\t\t\t<" + XML_Y + ">" + indexPosition.Y + "</" + XML_Y + ">");
						xml.Append("\n\t\t\t</Position>");
						xml.Append("\n\t\t</Guard>");
						break;
					default:
						if (this.mappingState != MappingState.None) {
							xml.Append("\t\t<" + this.mappingState + ">");
							xml.Append("\n\t\t\t<" + XML_X + ">" + indexPosition.X + "</" + XML_X + ">");
							xml.Append("\n\t\t\t<" + XML_Y + ">" + indexPosition.Y + "</" + XML_Y + ">");
							xml.Append("\n\t\t</" + this.mappingState + ">");
						}
						break;
				}

				if (this.mappingState != MappingState.None) {
					ScriptManager.getInstance().log(xml.ToString());
				}
			}
		}
		#endregion Support methods
	}
}
