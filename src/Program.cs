// Not.exe suppresses lines of input that start with, end with, or contain the specified string.
// Copyright( C ) 2023 Timothy J. Bruce

/*
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

namespace Icod.Not {

	public static class Program {

		#region nested classes
		private enum Mode {
			Undefined = 0,
			StartsWith,
			Contains,
			EndsWith
		}
		#endregion nested classes


		private const System.Int32 theBufferSize = 16384;

		[System.STAThread]
		public static System.Int32 Main( System.String[] args ) {
			var len = args.Length;
			if ( 10 < len ) {
				PrintUsage();
				return 1;
			}

			var processor = new Icod.Argh.Processor(
				new Icod.Argh.Definition[] {
					new Icod.Argh.Definition( "help", new System.String[] { "-h", "--help", "/help" } ),
					new Icod.Argh.Definition( "copyright", new System.String[] { "-c", "--copyright", "/copyright" } ),
					new Icod.Argh.Definition( "input", new System.String[] { "-i", "--input", "/input" } ),
					new Icod.Argh.Definition( "output", new System.String[] { "-o", "--output", "/output" } ),
					new Icod.Argh.Definition( "string", new System.String[] { "-s", "--string", "/string" } ),
					new Icod.Argh.Definition( "mode", new System.String[] { "-m", "--mode", "/mode" } ),
					new Icod.Argh.Definition( "compare", new System.String[] { "-cmp", "--compare", "/compare" } ),
				},
				System.StringComparer.OrdinalIgnoreCase
			);
			processor.Parse( args );

			if ( processor.Contains( "help" ) ) {
				PrintUsage();
				return 1;
			} else if ( processor.Contains( "copyright" ) ) {
				PrintCopyright();
				return 1;
			}

			if (
				( !processor.TryGetValue( "string", false, out var @string ) )
				|| System.String.IsNullOrEmpty( @string )
			) {
				PrintUsage();
				return 1;
			}

			if (
				( !processor.TryGetValue( "mode", false, out var modeStr ) )
				|| ( System.String.IsNullOrEmpty( modeStr ) )
			) {
				PrintUsage();
				return 1;
			}
			if ( !System.Enum.TryParse( typeof( Mode ), modeStr, true, out var mode ) ) {
				PrintUsage();
				return 1;
			}

			System.Func<System.String?, System.Collections.Generic.IEnumerable<System.String>> reader;
			if ( processor.TryGetValue( "input", true, out var inputPathName ) ) {
				if ( System.String.IsNullOrEmpty( inputPathName ) ) {
					PrintUsage();
					return 1;
				} else {
					reader = a => ReadFile( a! );
				}
			} else {
				reader = a => ReadStdIn();
			}

			System.Action<System.String?, System.Collections.Generic.IEnumerable<System.String>> writer;
			if ( processor.TryGetValue( "output", true, out var outputPathName ) ) {
				if ( System.String.IsNullOrEmpty( outputPathName ) ) {
					PrintUsage();
					return 1;
				} else {
					writer = ( a, b ) => WriteFile( a!, b );
				}
			} else {
				writer = ( a, b ) => WriteStdOut( b );
			}

			if ( !processor.TryGetValue( "compare", true, out var compareStr ) ) {
				compareStr = "CurrentCulture";
			}
			if (
				System.String.IsNullOrEmpty( compareStr )
				|| ( !System.Enum.TryParse( typeof( System.StringComparison ), compareStr, out var compare ) )
			) {
				PrintUsage();
				return 1;
			}

			System.Func<System.String, System.String, System.StringComparison, System.Boolean> worker;
			switch ( mode ) {
				case Mode.StartsWith:
					worker = ( a, b, c ) => a.StartsWith( b, c );
					break;
				case Mode.Contains:
					worker = ( a, b, c ) => ( 0 <= a.IndexOf( b, c ) );
					break;
				case Mode.EndsWith:
					worker = ( a, b, c ) => a.EndsWith( b, c );
					break;
				default:
					PrintUsage();
					return 1;
			}

			writer(
				outputPathName,
				reader( inputPathName ).Where(
					x => !worker( x, @string, (System.StringComparison)compare )
				)
			);
			return 0;
		}

		private static void PrintUsage() {
			System.Console.Error.WriteLine( "No, no, no! Use it like this, Einstein:" );
			System.Console.Error.WriteLine( "Not.exe --help" );
			System.Console.Error.WriteLine( "Not.exe --copyright" );
			System.Console.Error.WriteLine( "Not.exe (-s | --string | /string) theString (-m | --mode | /mode) (StartsWith | Contains | EndsWith) [(-cmd | --compare | /compere) (CurrentCulture | CurrentCultureIgnoreCase | InvariantCulture | InvariantCultureIgnoreCase | Ordinal | OrdinalIgnoreCase)] [(-i | --input | /input) inputFilePathName] [(-o | --output | /output) outputFilePathName]" );
			System.Console.Error.WriteLine( "Not.exe suppresses lines of input that start with, end with, or contain the specified string." );
			System.Console.Error.WriteLine( "The default value for the --compare switch is CurrentCulture." );
			System.Console.Error.WriteLine( "inputFilePathName and outputFilePathName may be relative or absolute paths." );
			System.Console.Error.WriteLine( "If inputFilePathName is omitted then input is read from StdIn." );
			System.Console.Error.WriteLine( "If outputFilePathName is omitted then output is written to StdOut." );
		}
		private static void PrintCopyright() {
			var copy = new System.String[] {
				"Not.exe suppresses lines of input that start with, end with, or contain the specified string.",
				"Copyright( C ) 2023 Timothy J. Bruce",
				"",
				"This program is free software: you can redistribute it and / or modify",
				"it under the terms of the GNU General Public License as published by",
				"the Free Software Foundation, either version 3 of the License, or",
				"( at your option ) any later version.",
				"",
				"This program is distributed in the hope that it will be useful,",
				"but WITHOUT ANY WARRANTY; without even the implied warranty of",
				"MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the",
				"GNU General Public License for more details.",
				"",
				"You should have received a copy of the GNU General Public License",
				"along with this program.If not, see <https://www.gnu.org/licenses/>."
			};
			foreach ( var line in copy ) {
				System.Console.WriteLine( line );
			}
		}

		#region io
		private static System.Collections.Generic.IEnumerable<System.String> ReadStdIn() {
			var line = System.Console.In.ReadLine();
			while ( null != line ) {
				line = line.TrimToNull();
				if ( null != line ) {
					yield return line;
				}
				line = System.Console.In.ReadLine();
			}
		}
		private static System.Collections.Generic.IEnumerable<System.String> ReadFile( System.String filePathName ) {
			using ( var file = System.IO.File.Open( filePathName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read ) ) {
				using ( var reader = new System.IO.StreamReader( file, System.Text.Encoding.UTF8, true, theBufferSize, true ) ) {
					var line = reader.ReadLine();
					while ( null != line ) {
						line = line.TrimToNull();
						if ( null != line ) {
							yield return line;
						}
						line = reader.ReadLine();
					}
				}
			}
		}

		private static void WriteStdOut( System.Collections.Generic.IEnumerable<System.String> data ) {
			foreach ( var datum in data ) {
				System.Console.Out.WriteLine( datum );
			}
		}
		private static void WriteFile( System.String filePathName, System.Collections.Generic.IEnumerable<System.String> data ) {
			using ( var file = System.IO.File.Open( filePathName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, System.IO.FileShare.None ) ) {
				_ = file.Seek( 0, System.IO.SeekOrigin.Begin );
				using ( var writer = new System.IO.StreamWriter( file, System.Text.Encoding.UTF8, theBufferSize, true ) ) {
					foreach ( var datum in data ) {
						writer.WriteLine( datum );
					}
					writer.Flush();
				}
				file.Flush();
				file.SetLength( file.Position );
			}
		}
		#endregion io

		private static System.String? TrimToNull( this System.String? @string ) {
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			}
			@string = @string.Trim();
			return System.String.IsNullOrEmpty( @string )
				? null
				: @string
			;
		}

	}

}