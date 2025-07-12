#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdint.h>
#include <unistd.h>

#define INTERACTIVE 1
#define INTERACTIVE_TIME_FLOORSTART 50*1000
#define INTERACTIVE_TIME_WALLDRAWING 5*1000
#define INTERACTIVE_TIME_WALLEND 16*1000
#define INTERACTIVE_TIME_FLOOREND 400*1000

// Floor size
#define FH 9
#define FW 18

// Map data. This indicates the space side, and each space shows the presence or 
// absence of the pole at the bottom right, and the right and bottom walls.
// bit1 = the reached point
// bit2 = presence or absence of the right wall 
// bit3 = presence or absence of the bottom wall
uint8_t mapData[FH][FW];
uint8_t mapData2[FH][FW];

uint8_t seed = -1;
uint8_t seed0 = -1;
uint8_t prev;

/**
 * Clear the `mapData` array.
 */
void resetMap() {
  memset(mapData, 0, sizeof(mapData));
}

void showMap(void) {
  printf("FLOOR %d (seed=%d)\n", ( (int8_t)(seed0) + 60) % 60 + 1, seed0);
  printf("▗");
  for (int x = 0; x < FW; x++) {
    printf("▄");
  }
  printf("\n");
  for (int y = 0; y < FH; y++) {
    printf("▐");
    for (int x = 0; x < FW; x++) {
      uint8_t c = mapData[y][x] >> 1;
      if (y == FH - 1) { c |= 2; }
      if (x == FW - 1) { c |= 1; }
      switch (c) {
      case 0: printf("▗"); break;
      case 1: printf("▐"); break;
      case 2: printf("▄"); break;
      case 3: printf("▟"); break;
      default: printf(" "); break;
      }
    }
    printf("\t");
    for (int x = 0; x < FW; x++) {
      printf("%1d", (mapData[y][x]));
    }
    printf("\n");
  }
}
void showSequence(uint8_t *sequence, int seq) {
  printf("\x1b[2m");
  for (int i = 0; i < 200; i++) {
    if (i == seq) {   printf("\x1b[0m"); }
    printf("%1d", sequence[i]);
    if (i % 40 == 39) printf("\n");
  }
  printf("\n");
}

/**
 * Check if there is a wall in the direction indicated by `dir` from the pole 
 * at the bottom right of x, y.
 * `dir`: Direction (0: up, 1: right, 2: down, 3: left)
 */
int checkWall(int x, int y, int dir) {
  switch (dir) {
  case 0:
    return (mapData[y][x] & 2) != 0;
  case 1:
    if (x >= FW - 1) { return 1; }
    return (mapData[y][x + 1] & 4) != 0;
  case 2:
    if (y >= FH - 1) { return 1; }
    return (mapData[y + 1][x] & 2) != 0;
  case 3:
    return (mapData[y][x] & 4) != 0;
  default:
    return 0;
  }
}

/**
 * Return whether the pole at x, y has been checked.
 * If it has not been checked, mark it as checked.
 * @return
 * 1 if the pole has already been checked or it is an outer wall.
 */
int checkAndMarkPole(int x, int y) {
  if (x < 0 || x >= FW - 1) return 1;
  if (y < 0 || y >= FH - 1) return 1;
  int marked = (mapData[y][x] & 1) != 0;
  return marked;
}

/**
 * Extend the wall in the direction indicated by `dir` from the pole 
 * at the bottom right of x, y.
 * `dir`: Direction (0: up, 1: right, 2: down, 3: left)
 * @return
 * 1 when the wall extension is finished (if the pole at the extended 
 * end has already been checked).
 */
int makeWall(int *x, int *y, int dir) {
  mapData[*y][*x] |= 1;
  if (checkWall(*x, *y, dir)) return 0; // When there is already a wall, return 0
  switch (dir) {
  case 0:
    mapData[*y][*x] |= 2;
    *y = *y - 1;
    return checkAndMarkPole(*x, *y);
  case 1:
    if (*x >= FW - 1) { return 1; } // Basically this should not happen
    mapData[*y][*x + 1] |= 4;
    *x = *x + 1;
    return checkAndMarkPole(*x, *y);
  case 2:
    if (*y >= FH - 1) { return 1; } // Basically this should not happen
    mapData[*y + 1][*x] |= 2;
    *y = *y + 1;
    return checkAndMarkPole(*x, *y);
  case 3:
    mapData[*y][*x] |= 4;
    *x = *x - 1;
    return checkAndMarkPole(*x, *y);
  default:
    return 1;
  }
}

#define SEQLEN 1024
uint8_t sequence[SEQLEN];

uint8_t next() {
  uint8_t r1 = ((seed >> 7) ^ (seed >> 4)) & 0x01;
  uint8_t r2 = r1 ^ 1;
  seed = (uint8_t)(seed << 1) | r2;
  r1 = (uint8_t)((prev << 1) | r2) & 0x3;
  prev = r2;
  return r1;
}

int main(int argc, char **argv) {
  if (argc >= 2) {
    seed = atoi(argv[1]);
  }
  seed0 = seed;
  prev = seed & 1;
  
  for (int i = 0; i < SEQLEN; i++) { 
    sequence[i] = next();
  }

  // Make the maze.
  resetMap();
  int seq = 0;
  int step = 0;
  if (INTERACTIVE) {
    printf("\033[2J\033[1;1H");
  }
  showMap(); printf("\n\n"); showSequence(sequence, seq);

  if (INTERACTIVE) { usleep(INTERACTIVE_TIME_FLOORSTART); }
  for (int x = FW - 2; x >= 0; x--) {
    for (int y = 0; y < FH - 1; y++) {
      /* printf("x %d, y %d\n", x, y); */
      int cx = x, cy = y;
      int checked = checkAndMarkPole(cx, cy);
      int proceed = (!checked);
      while (!checked) {
	checked = makeWall(&cx, &cy, sequence[seq++]);
	if (INTERACTIVE) {
	  usleep(INTERACTIVE_TIME_WALLDRAWING);
	  printf("\033[2J\033[1;1H");
	}
	showMap(); 
	printf("\nstep %d, seq %d, cx %d, cy %d\n", step++, seq, cx, cy);
	showSequence(sequence, seq);
      }
      if (INTERACTIVE && proceed) { usleep(INTERACTIVE_TIME_WALLEND); }

      /* if (proceed) { */
      /* 	printf("step %d, seq %d, x %d, y %d\n", step++, seq, x, y); */
      /* 	showMap(sequence+seq); */
      /* } */
    }
  }

  if (INTERACTIVE) {
    usleep(INTERACTIVE_TIME_FLOOREND);
  }
  
  return 0;
}