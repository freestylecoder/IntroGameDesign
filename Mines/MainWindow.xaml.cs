using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MineFind {
	public partial class MainWindow : Window {
		private const int MINE = 9;

		private const int MAX_MINE_COUNT = 10;
		private const int MINE_FIELD_WIDTH = 10;
		private const int MINE_FIELD_HEIGHT = 10;
		private const int MINE_FIELD_SIZE = MINE_FIELD_WIDTH * MINE_FIELD_HEIGHT;

		private int FoundMines;
		private int[,] MineArray;

		private bool GameStarted;
		private DateTime GameStop = DateTime.MinValue;
		private DateTime GameStart = DateTime.MinValue;

		public MainWindow() {
			InitializeComponent();

			FoundMines = 0;
			GameStarted = false;

			Loaded += PageLoaded;
			CompositionTarget.Rendering += Draw;
		}

		private void Draw( object sender, EventArgs e ) {
			if( GameStarted )
				Stopwatch.Text = ( DateTime.Now - GameStart ).ToString( "mm\\:ss\\.fff" );
			else
				Stopwatch.Text = ( GameStop - GameStart ).ToString( "mm\\:ss\\.fff" );

			MineCount.Text = string.Format( "{0:D3}", MAX_MINE_COUNT - FoundMines );
		}

		private void PageLoaded( object sender, RoutedEventArgs e ) {
			#region Load the Row Definitions
			for( int _lcv = 0; _lcv < MINE_FIELD_HEIGHT; ++_lcv ) {
				RowDefinition _RowDefinition = new RowDefinition();
				_RowDefinition.Height = GridLength.Auto;
				MineField.RowDefinitions.Add( _RowDefinition );
			}
			#endregion

			#region Load the Column Definitions
			for( int _lcv = 0; _lcv < MINE_FIELD_WIDTH; ++_lcv ) {
				ColumnDefinition _ColumnDefinition = new ColumnDefinition();
				_ColumnDefinition.Width = GridLength.Auto;
				MineField.ColumnDefinitions.Add( _ColumnDefinition );
			}
			#endregion

			#region Create the Buttons
			for( int _Y = 0; _Y < MINE_FIELD_HEIGHT; ++_Y ) {
				for( int _X = 0; _X < MINE_FIELD_WIDTH; ++_X ) {
					Button _NewButton = new Button();
					_NewButton.Tag = 0;
					_NewButton.Click += ButtonClicked;
					_NewButton.MouseRightButtonUp += PlantFlag;
					_NewButton.Name = string.Format( "X{0}Y{1}", _X, _Y );
					_NewButton.Width = MineField.ActualWidth / MINE_FIELD_WIDTH;
					_NewButton.Height = MineField.ActualHeight / MINE_FIELD_HEIGHT;

					Grid.SetRow( _NewButton, _Y );
					Grid.SetColumn( _NewButton, _X );

					MineField.Children.Add( _NewButton );
				}
			}
			#endregion
		}

		private Button FindButton( int X, int Y ) {
			foreach( UIElement _Button in MineField.Children ) {
				if( string.Format( "X{0}Y{1}", X, Y ) == ( _Button as FrameworkElement ).Name ) {
					return _Button as Button;
				}
			}

			return null;
		}

		private void ButtonClicked( object sender, RoutedEventArgs e ) {
			if( GameStarted ) {
				// Get the button
				Button _ClickedButton = sender as Button;
				if( null == _ClickedButton )
					return;

				// If we're here, the Image has to be the flag.
				if( _ClickedButton.Content is Image )
					return;

				if( MINE == (int)_ClickedButton.Tag ) {
					#region Display Bomb
					BitmapImage _BombBitmapImage = new BitmapImage();
					_BombBitmapImage.BeginInit();
					_BombBitmapImage.UriSource = new Uri( @"./Assets/Bomb.png", UriKind.Relative );
					_BombBitmapImage.DecodePixelWidth = 16;
					_BombBitmapImage.EndInit();

					Image Bomb = new Image();
					Bomb.Width = 16;
					Bomb.Source = _BombBitmapImage;

					_ClickedButton.Content = Bomb;
					#endregion

					#region End Game
					GameStarted = false;
					GameStop = DateTime.Now;
					MessageBox.Show( "YOU LOSE!", "Mine Find", MessageBoxButton.OK, MessageBoxImage.Error );
					#endregion

					return;
				}

				_ClickedButton.Content = (int)_ClickedButton.Tag;

				#region Open up nearby empty buttons
				if( 0 == (int)_ClickedButton.Tag ) {
					for( int X = Grid.GetColumn( _ClickedButton ) - 1; X < Grid.GetColumn( _ClickedButton ) + 2; ++X ) {
						for( int Y = Grid.GetRow( _ClickedButton ) - 1; Y < Grid.GetRow( _ClickedButton ) + 2; ++Y ) {
							Button _ButtonToCheck = FindButton( X, Y );
							if( null != _ButtonToCheck ) {
								if( null == _ButtonToCheck.Content ) {
									ButtonClicked( _ButtonToCheck, null );
								}
							}
						}
					}
				}
				#endregion
			}
		}

		private void PlantFlag( object sender, System.Windows.Input.MouseButtonEventArgs e ) {
			if( GameStarted ) {
				Button _ClickedButton = sender as Button;
				if( null == _ClickedButton )
					return;

				if( null == _ClickedButton.Content ) {
					if( MAX_MINE_COUNT != FoundMines ) {
						++FoundMines;

						#region Plant the Flag
						BitmapImage _FlagBitmapImage = new BitmapImage();
						_FlagBitmapImage.BeginInit();
						_FlagBitmapImage.UriSource = new Uri( @"./Assets/Flag.png", UriKind.Relative );
						_FlagBitmapImage.DecodePixelWidth = 16;
						_FlagBitmapImage.EndInit();

						Image Flag = new Image();
						Flag.Width = 16;
						Flag.Source = _FlagBitmapImage;
						_ClickedButton.Content = Flag;
						#endregion
					}

					if( MAX_MINE_COUNT == FoundMines ) {
						#region See if all the flags are on bombs
						int FlagsOnBombs = 0;
						for( int X = 0; X < MINE_FIELD_WIDTH; ++X ) {
							for( int Y = 0; Y < MINE_FIELD_HEIGHT; ++Y ) {
								Button _ButtonToCheck = FindButton( X, Y );
								if( null != _ButtonToCheck ) {
									if( MINE == (int)_ButtonToCheck.Tag ) {
										if( _ButtonToCheck.Content is Image ) {
											++FlagsOnBombs;
										}
									}
								}
							}
						}

						#region YOU WIN
						if( MAX_MINE_COUNT == FlagsOnBombs ) {
							GameStarted = false;
							GameStop = DateTime.Now;
							MessageBox.Show( "YOU WIN!", "Mine Find", MessageBoxButton.OK, MessageBoxImage.None );
						}
						#endregion
						#endregion
					}
				} else if( _ClickedButton.Content is Image ) {
					--FoundMines;
					_ClickedButton.Content = null;
				} else if( _ClickedButton.Content is int ) {
					#region Count flags touching button
					int TouchingFlags = 0;
					for( int X = Grid.GetColumn( _ClickedButton ) - 1; X < Grid.GetColumn( _ClickedButton ) + 2; ++X ) {
						for( int Y = Grid.GetRow( _ClickedButton ) - 1; Y < Grid.GetRow( _ClickedButton ) + 2; ++Y ) {
							Button _ButtonToCheck = FindButton( X, Y );
							if( null != _ButtonToCheck ) {
								if( _ButtonToCheck.Content is Image ) {
									++TouchingFlags;
								}
							}
						}
					}
					#endregion

					#region If count matches, open nearby cells
					if( (int)_ClickedButton.Content == TouchingFlags ) {
						for( int X = Grid.GetColumn( _ClickedButton ) - 1; X < Grid.GetColumn( _ClickedButton ) + 2; ++X ) {
							for( int Y = Grid.GetRow( _ClickedButton ) - 1; Y < Grid.GetRow( _ClickedButton ) + 2; ++Y ) {
								Button _ButtonToCheck = FindButton( X, Y );
								if( null != _ButtonToCheck ) {
									if( null == _ButtonToCheck.Content ) {
										ButtonClicked( _ButtonToCheck, null );
									}
								}
							}
						}
					}
					#endregion
				}
			}
		}

		private void InitGame( object sender, RoutedEventArgs e ) {
			int _PossibeLocation;
			Random _Random = new Random();
			List<int> _MineLocations = new List<int>( MAX_MINE_COUNT );

			#region Randomly choose mine locations
			while( MAX_MINE_COUNT > _MineLocations.Count ) {
				_PossibeLocation = _Random.Next( 0, MINE_FIELD_SIZE );
				if( !_MineLocations.Contains( _PossibeLocation ) )
					_MineLocations.Add( _PossibeLocation );
			}
			#endregion

			#region Place Mines in array
			MineArray = new int[MINE_FIELD_WIDTH, MINE_FIELD_HEIGHT];
			Parallel.ForEach( _MineLocations, ( _MineLocation ) => {
				MineArray[_MineLocation % MINE_FIELD_WIDTH, _MineLocation / MINE_FIELD_WIDTH] = MINE;
			} );
			#endregion

			#region Fill array with touching counts
			Parallel.For( 0, MINE_FIELD_HEIGHT, ( Y ) => {
				Parallel.For( 0, MINE_FIELD_WIDTH, ( X ) => {
					if( MINE == MineArray[X, Y] ) {
						Parallel.For( Clamp( Y - 1, 0, MINE_FIELD_HEIGHT ), Clamp( Y + 2, 0, MINE_FIELD_HEIGHT ), ( NeighborY ) => {
							Parallel.For( Clamp( X - 1, 0, MINE_FIELD_WIDTH ), Clamp( X + 2, 0, MINE_FIELD_WIDTH ), ( NeighborX ) => {
								if( MINE != MineArray[NeighborX, NeighborY] )
									++MineArray[NeighborX, NeighborY];
							} );
						} );
					}
				} );
			} );
			#endregion

			#region Add mine array data to buttons
			foreach( Button _Button in MineField.Children ) {
				int X = Grid.GetColumn( _Button );
				int Y = Grid.GetRow( _Button );

				_Button.Tag = MineArray[X, Y];
				_Button.Content = null;
			}
			#endregion

			FoundMines = 0;
			GameStarted = true;
			GameStart = DateTime.Now;
		}

		private static int Clamp( int Value, int MinValue, int MaxValue ) {
			if( Value < MinValue ) return MinValue;
			if( Value > MaxValue ) return MaxValue;
			return Value;
		}
	}
}
