import pandas as pd
import spacy
from sklearn.model_selection import train_test_split
from spacy.util import minibatch
import matplotlib.pyplot as plt
from sklearn.metrics import confusion_matrix, ConfusionMatrixDisplay
from spacy.training import Example
from sklearn.metrics import precision_score, recall_score, f1_score

df = pd.read_csv('./Dataset/dataset_28.csv', quotechar='"', on_bad_lines='skip', encoding="utf-8")

# Вывод названий всех колонок
print(df.columns)
# Загрузка датасета из CSV файла
df = pd.read_csv('./Dataset/dataset_28.csv', on_bad_lines='skip', encoding="utf-8")

# Оставляем только те строки, где есть разметка (без пустых значений в колонке 'sentiment')


# Преобразуем данные в подходящий формат: положительные метки в 1, отрицательные в 0
df['sentiment'] = df['sentiment'].apply(lambda x: 1 if x == 'Positive' else 0)

# Оставляем только необходимые колонки для обучения: текст и метка
df = df[['text', 'sentiment']]

train_data, test_data = train_test_split(df, test_size=0.2, random_state=42)


def convert_to_spacy_format(data):
    return [(row['text'], {"cats": {"POSITIVE": row['sentiment'] == 1, "NEGATIVE": row['sentiment'] == 0}}) for _, row
            in data.iterrows()]


train_data = convert_to_spacy_format(train_data)
test_data = convert_to_spacy_format(test_data)



nlp = spacy.blank("ru")
# Добавляем pipeline для текстовой классификации, если его еще нет
if "textcat" not in nlp.pipe_names:
    textcat = nlp.add_pipe("textcat", last=True)

# Определяем метки для классификации: позитив и негатив
textcat.add_label("POSITIVE")
textcat.add_label("NEGATIVE")

# Количество эпох обучения
n_iter = 12

# Начинаем обучение и получаем оптимизатор
optimizer = nlp.begin_training()

# Цикл обучения
for i in range(n_iter):
    losses = {}

    # Разбиваем данные на мини-батчи
    batches = minibatch(train_data, size=8)

    for batch in batches:
        examples = []
        for text, annotations in batch:
            # Создаем пример для обучения
            doc = nlp.make_doc(text)
            example = Example.from_dict(doc, annotations)
            examples.append(example)

        # Обновляем модель
        nlp.update(examples, sgd=optimizer, losses=losses)

    print(f"Losses at iteration {i}: {losses}")

correct = 0
total = 0

for text, annotations in test_data:
    doc = nlp(text)
    predicted_label = doc.cats['POSITIVE'] >= 0.5  # Если вероятность >= 0.5, то это позитив
    true_label = annotations['cats']['POSITIVE'] == 1  # Истинная метка
    if predicted_label == true_label:
        correct += 1
    total += 1

accuracy = correct / total
print(total, correct)
print(f"Точность модели: {accuracy}")

# Сохраняем модель в директорию
#nlp.to_disk("ru_sentiment_model")

#print("Модель успешно сохранена!")




# Собираем истинные и предсказанные метки на тестовом наборе
y_true = []
y_pred = []

for text, annotations in test_data:
    doc = nlp(text)
    predicted_label = 1 if doc.cats['POSITIVE'] >= 0.5 else 0
    true_label = 1 if annotations['cats']['POSITIVE'] else 0
    y_true.append(true_label)
    y_pred.append(predicted_label)

# Вычисляем матрицу ошибок
cm = confusion_matrix(y_true, y_pred, labels=[1, 0])

# Визуализируем матрицу ошибок
disp = ConfusionMatrixDisplay(confusion_matrix=cm, display_labels=['Positive', 'Negative'])
disp.plot(cmap=plt.cm.Blues)
plt.title('Матрица ошибок модели')
plt.show()



# Загрузка датасета
df = pd.read_csv('./Dataset/dataset_28.csv', on_bad_lines='skip', encoding="utf-8")

# Предобработка данных:
# Преобразуем метки в числовой формат: Positive -> 1, Negative -> 0
df = df.dropna(subset=['sentiment'])  # Убираем строки без разметки
df['sentiment'] = df['sentiment'].apply(lambda x: 1 if x == 'Positive' else 0)

# Оставляем только необходимые колонки
df = df[['text', 'sentiment']]

# Разделение на обучающую и тестовую выборки
train_df, test_df = train_test_split(df, test_size=0.2, random_state=42)

# Функция для преобразования данных в формат, понятный spaCy
def convert_to_spacy_format(dataframe):
    return [(row['text'], {"cats": {"POSITIVE": row['sentiment'] == 1, "NEGATIVE": row['sentiment'] == 0}})
            for _, row in dataframe.iterrows()]

train_data = convert_to_spacy_format(train_df)
test_data = convert_to_spacy_format(test_df)

# Создание пустой модели spaCy для русского языка
nlp = spacy.blank("ru")

# Добавление компонента текстовой классификации, если его ещё нет
if "textcat" not in nlp.pipe_names:
    textcat = nlp.add_pipe("textcat", last=True)
else:
    textcat = nlp.get_pipe("textcat")

# Добавление меток классов
textcat.add_label("POSITIVE")
textcat.add_label("NEGATIVE")

# Начало обучения модели
n_iter = 12
optimizer = nlp.begin_training()

for i in range(n_iter):
    losses = {}
    batches = minibatch(train_data, size=8)
    for batch in batches:
        examples = []
        for text, annotations in batch:
            doc = nlp.make_doc(text)
            example = Example.from_dict(doc, annotations)
            examples.append(example)
        nlp.update(examples, sgd=optimizer, losses=losses)
    print(f"Losses at iteration {i + 1}: {losses}")

# Оценка модели на тестовой выборке
y_true = []
y_pred = []

for text, annotations in test_data:
    doc = nlp(text)
    predicted_label = 1 if doc.cats['POSITIVE'] >= 0.5 else 0
    true_label = 1 if annotations['cats']['POSITIVE'] else 0
    y_pred.append(predicted_label)
    y_true.append(true_label)

# Вычисление метрик классификации
from sklearn.metrics import precision_score, recall_score, f1_score

precision = precision_score(y_true, y_pred)
recall = recall_score(y_true, y_pred)
f1 = f1_score(y_true, y_pred)
accuracy = sum([1 for yt, yp in zip(y_true, y_pred) if yt == yp]) / len(y_true)

# Вывод таблицы с форматированием
print("+------------+-----------+")
print("| Метрика    | Значение  |")
print("+------------+-----------+")
print("| Accuracy   | {:9.4f} |".format(accuracy))
print("| Precision  | {:9.4f} |".format(precision))
print("| Recall     | {:9.4f} |".format(recall))
print("| F1-score   | {:9.4f} |".format(f1))
print("+------------+-----------+")
