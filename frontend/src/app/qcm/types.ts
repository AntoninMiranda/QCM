// src/app/qcm/types.ts

export interface QcmQuestion {
  id: number;
  question: string;
  choices: { [key: string]: string };
  answer: string;
}

export interface QcmQuestionGroup {
  annexes: string;
  questions: QcmQuestion[];
}

export interface QcmResult {
  score: number;
  total: number;
}

export interface QuestionDetail {
  questionId: number;
  question: string;
  userAnswer: string | null;
  correctAnswer: string;
  isCorrect: boolean;
}
