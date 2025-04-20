import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { SongInterface } from '../interfaces/songs/song.interface';

@Injectable({
  providedIn: 'root'
})
export class AudioPlayerService {
  private readonly currentSongSubject$: BehaviorSubject<SongInterface> = new BehaviorSubject<SongInterface>(null);
  public readonly currentSong$: Observable<SongInterface> = this.currentSongSubject$.asObservable();

  private readonly bufferSongsSubject$: BehaviorSubject<SongInterface[]> = new BehaviorSubject<SongInterface[]>(null);
  public readonly bufferSongs$: Observable<SongInterface[]> = this.bufferSongsSubject$.asObservable();
  
  constructor() { }

  public setCurrentSong(song: SongInterface): void {
    this.currentSongSubject$.next(song);
  }

  public setSongs(bufferSongs: SongInterface[]): void {
    this.bufferSongsSubject$.next(bufferSongs);
  }
}
