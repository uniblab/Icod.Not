# Icod.Not
Not.exe suppresses lines of input that start with, end with, or contain the specified string.

## Usage
`Not.exe --help`
Displays this text.

`Not.exe --copyright`
Displays copyright and licensing information.

`Not.exe --string theString --mode (StartsWith | Contains | EndsWith) [--compare (CurrentCulture | CurrentCultureIgnoreCase | InvariantCulture | InvariantCultureIgnoreCase | Ordinal | OrdinalIgnoreCase)] [--input inputFilePathName] [--output outputFilePathName]`
Not.exe suppresses lines of input that start with, end with, or contain the specified string.
The default value for the --compare switch is CurrentCulture.
inputFilePathName and outputFilePathName may be relative or absolute paths.
If inputFilePathName is omitted then input is read from StdIn.
If outputFilePathName is omitted then output is written to StdOut.

Not.exe does not use Regular Expressions.

## Copyright and Licensing
Not.exe suppresses lines of input that start with, end with, or contain the specified string.
Copyright( C ) 2023 Timothy J. Bruce

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published 
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.

