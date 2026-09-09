"""Build current corrected assets with Blender; overwrites generated files."""
from pathlib import Path
import runpy
folder=Path(__file__).resolve().parent
for script in ('build_jimothy.py','build_ballard.py'):
 runpy.run_path(str(folder/script),run_name='__main__')
