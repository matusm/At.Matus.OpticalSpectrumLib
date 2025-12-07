# OpticalSpectrumLib

A small, focused .NET library for representing and working with optical spectra and individual spectral samples.

## Overview
`OpticalSpectrumLib` models spectral data as individual points and collections, provides simple statistical fields, basic spectrum math helpers, masking/filtering utilities, and CSV export support for analysis and desktop workflows.

## Key types
- `ISpectralPoint`, `SpectralPoint`, `MeasuredSpectralPoint` — per-wavelength data (wavelength, signal, SEM, StdDev).
- `IOpticalSpectrum`, `OpticalSpectrum`, `MeasuredOpticalSpectrum` — collections of spectral points and operations on them.
- `SpecMath` — small math helpers for spectra.
- `Masker` — masking and filtering operations for spectra.
- `Resampler` — resample spectra to regular spaced (or any other set) wavelength values.

## CSV
Spectral points can be exported to CSV using `ToCsvLine()` and a header is available via `GetCsvHeader()`:
- Header: `Wavelength,Signal,StdErr`
- Example CSV line: values formatted as `Wavelength,Signal,StdErr:`
