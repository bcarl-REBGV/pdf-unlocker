# -*- mode: python ; coding: utf-8 -*-

from kivy_deps import sdl2, glew

block_cipher = None


a = Analysis(
    ['main.py'],
    pathex=[],
    binaries=[],
    datas = [('./assets/locked-padlock.png', './assets'), ('./assets/unlocked-padlock.png', './assets'), ('./assets/spinner.gif', './assets')],
    hiddenimports=[],
    hookspath=[],
    hooksconfig={},
    runtime_hooks=[],
    excludes=[],
    win_no_prefer_redirects=False,
    win_private_assemblies=False,
    cipher=block_cipher,
    noarchive=False,
)

# a.datas += Tree('./assets', prefix="dir_to_include")

pyz = PYZ(a.pure, a.zipped_data, cipher=block_cipher)

exe = EXE(
    *[Tree(p) for p in (sdl2.dep_bins + glew.dep_bins)],
    pyz,
    a.scripts,
    a.binaries,
    a.zipfiles,
    a.datas,
    [],
    name='PDFUnlocker',
    debug=False,
    bootloader_ignore_signals=False,
    strip=False,
    upx=True,
    upx_exclude=[],
    runtime_tmpdir=None,
    console=False,
    disable_windowed_traceback=False,
    argv_emulation=False,
    target_arch=None,
    codesign_identity=None,
    entitlements_file=None,
)
