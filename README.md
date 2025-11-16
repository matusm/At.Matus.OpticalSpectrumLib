# OpticalSpectrumLib

A small, focused .NET library for representing and working with optical spectra and individual spectral samples.

## Overview
`OpticalSpectrumLib` models spectral data as individual points and collections, provides simple statistical fields, basic spectrum math helpers, masking/filtering utilities, and CSV export support for analysis and desktop workflows.

## Key types
- `ISpectralPoint`, `SpectralPoint`, `MeasuredSpectralPoint` — per-wavelength data (wavelength, signal, SEM, StdDev).
- `IOpticalSpectrum`, `OpticalSpectrum`, `MeasuredOpticalSpectrum` — collections of spectral points and operations on them.
- `SpecMath` — small math helpers for spectra.
- `Masker` — masking and filtering operations for spectra.

## CSV
Spectral points can be exported to CSV using `ToCsvLine()` and a header is available via `GetCsvHeader()`:
- Header: `Wavelength,Signal,SEM,StdDev`
- Example CSV line: values formatted as `Wavelength:F2,Signal:F6,StdErr:F6,StdDev:F6`

## Quick start
1. Open the solution in Visual Studio 2022 via __File > Open > Project/Solution__.
2. Build the solution via __Build > Build Solution__.
3. Example usage: